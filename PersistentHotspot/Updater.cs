using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersistentHotspot
{
    public static class Updater
    {
        private static string github_user, product_name;
        private static int updatecheck_interval_mins;

        public static void Run(string _product_name, int _updatecheck_interval_mins = 60, string _github_user = "ashvin-bhuttoo")
        {
            github_user = _github_user;
            product_name = _product_name;
            updatecheck_interval_mins = _updatecheck_interval_mins;

            Task.Run(async () =>
            {
                await UpdaterThreadAsync();
            });
        }

        /// <summary>
        /// A generic update checker for github projects
        /// The tag text is used as version field from github releases, the description text should contain an installer link pointed at raw.githubusercontent.com
        /// </summary>
        /// <returns></returns>
        private static async Task UpdaterThreadAsync()
        {
        retry:
            Octokit.GitHubClient client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(product_name));
            IReadOnlyList<Octokit.Release> rlsAll = await client.Repository.Release.GetAll(github_user, product_name);
            if (rlsAll != null && rlsAll.Count > 0)
            {
                Octokit.Release latest = rlsAll.OrderBy(r => r.CreatedAt).Last();

                Version latest_version = null;
                if (Version.TryParse(latest.TagName, out latest_version))
                {
                    if (Assembly.GetExecutingAssembly().GetName().Version < latest_version)
                    {
                        if (MessageBox.Show($"A New Version {latest.TagName} of {product_name} has been released, do you wish to update?", "New Version Available!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            string[] body = latest.Body.Split(new[] { '"' }, StringSplitOptions.RemoveEmptyEntries);

                            string installerUrl = string.Empty;
                            foreach (var tmp in body)
                            {
                                if (tmp.Contains("raw.githubusercontent.com"))
                                {
                                    installerUrl = tmp;
                                    break;
                                }
                            }

                            if (installerUrl != string.Empty)
                            {
                                if (File.Exists("update.msi"))
                                    File.Delete("update.msi");

                                using (var _client = new WebClient())
                                {
                                    _client.DownloadFile(installerUrl, "update.msi");
                                }

                                Process.Start("update.msi");
                                Application.Exit();
                                Environment.Exit(Environment.ExitCode);
                            }
                            else
                            {
                                Process.Start($"https://github.com/{github_user}/{product_name}/releases");
                            }
                        }
                    }
                }
            }

            await Task.Delay(updatecheck_interval_mins * 60000);
            goto retry;
        }
    }
}

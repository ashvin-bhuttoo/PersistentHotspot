# PersistentHotspot
Following a recent update in Windows 10, the Wifi Hotspot feature automatically turns off after a while if there are no connecting clients, there is no way to make it permanently ON anymore using windows configurations. As i wanted my hotspot to be permanently ON, here is PersistentHotspot! 

If you want to buy me a beer, here's the button for it.. :)<br/>
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://paypal.me/ABhuttoo?locale.x=en_US)

This project was built using VS2019 Community Edition and the Windows Kits Library was installed as it is a dependency.

The latest installer can be downloaded from here: <a href="https://raw.githubusercontent.com/ashvin-bhuttoo/PersistentHotspot/master/PersistentHotspotSetup/Release/PersistentHotspotSetup.msi"> v1.0.2</a>, uninstall older version before installing latest <br/> 
Old Versions: <a href="https://raw.githubusercontent.com/ashvin-bhuttoo/PersistentHotspot/master/PersistentHotspotSetup/Archived/PersistentHotspotSetup_v1.0.1.msi"> v1.0.1</a> <a href="https://raw.githubusercontent.com/ashvin-bhuttoo/PersistentHotspot/master/PersistentHotspotSetup/Archived/PersistentHotspotSetup_v1.0.0.msi"> v1.0.0</a>

PersistentHotspot uses the WindowsRuntime reference to access the Windows Runtime API that exposes the required interfaces such as NetworkOperatorTetheringManager.
Strangely, The only way to add a reference to Windows Runtime is to edit your .csproj manually and add the following references, why Microsoft why ?

```xml
<ItemGroup>
   ....
   ....
	<Reference Include="System.Runtime.WindowsRuntime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL">
	  <SpecificVersion>False</SpecificVersion>
	  <HintPath>$(MSBuildProgramFiles32)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll</HintPath>
	</Reference>
	<Reference Include="Windows">
	  <HintPath>$(MSBuildProgramFiles32)\Windows Kits\10\UnionMetadata\10.0.17763.0\Windows.winmd</HintPath>
	</Reference>
  </ItemGroup>
```

<b>Changelog</b>
```
v1.0.2
- Added Option to Auto Restart Hotspot at user defined interval in tray icon context menu
- PersistentHotspot will now remember the last state and resume that state after reboot
	i.e. If Hotspot was turned off, after a reboot, the hotspot will persist the Off state
- Fixed issue with PersistentHotspot randomly gets in the 'Unknown Error' state and stops working.

v1.0.1
- Added Exit button in tray icon context menu
- Added option to toggle the PersistentHotpot ON or OFF
	+OFF state will keep wifi hotpot off even if turned on manually
	+ON state will keep wifi hotspot on even if turned off manually
- Added desktop icon 
```

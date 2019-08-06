# PersistentHotspot
Following a recent update in Windows 10, the Wifi Hotspot feature automatically turns off after a while if there are no connecting clients, there is no way to make it permanently ON anymore using windows configurations. As i wanted my hotspot to be permanently ON, here is PersistentHotspot! 

This project was built using VS2019 Community Edition and the Windows Kits Library was installed as it is a dependency.

The installer can be downloaded from here:
https://github.com/ashvin-bhuttoo/PersistentHotspot/blob/master/PersistentHotspotSetup/Release/PersistentHotspotSetup.msi

PersistentHotspot uses the WindowsRuntime reference to access the Windows Runtime API that exposes the required interfaces such as NetworkOperatorTetheringManager.
Unfortunately, The only way to add a reference to Windows Runtime is to edit your .csproj manually and add the following references, why Microsoft why ?

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

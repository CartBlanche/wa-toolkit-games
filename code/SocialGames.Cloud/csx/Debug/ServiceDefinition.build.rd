<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="SocialGames.Cloud" generation="1" functional="0" release="0" Id="5b7d0128-4832-4781-b64f-2da8a97ef2e1" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="SocialGames.CloudGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="SocialGames.Web:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/LB:SocialGames.Web:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="SocialGames.Web:DataConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/MapSocialGames.Web:DataConnectionString" />
          </maps>
        </aCS>
        <aCS name="SocialGames.Web:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/MapSocialGames.Web:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="SocialGames.WebInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/MapSocialGames.WebInstances" />
          </maps>
        </aCS>
        <aCS name="SocialGames.Worker:DataConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/MapSocialGames.Worker:DataConnectionString" />
          </maps>
        </aCS>
        <aCS name="SocialGames.Worker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/MapSocialGames.Worker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="SocialGames.Worker:StatisticsConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/MapSocialGames.Worker:StatisticsConnectionString" />
          </maps>
        </aCS>
        <aCS name="SocialGames.WorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/MapSocialGames.WorkerInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:SocialGames.Web:Endpoint1">
          <toPorts>
            <inPortMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.Web/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapSocialGames.Web:DataConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.Web/DataConnectionString" />
          </setting>
        </map>
        <map name="MapSocialGames.Web:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.Web/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapSocialGames.WebInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.WebInstances" />
          </setting>
        </map>
        <map name="MapSocialGames.Worker:DataConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.Worker/DataConnectionString" />
          </setting>
        </map>
        <map name="MapSocialGames.Worker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.Worker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapSocialGames.Worker:StatisticsConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.Worker/StatisticsConnectionString" />
          </setting>
        </map>
        <map name="MapSocialGames.WorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.WorkerInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="SocialGames.Web" generation="1" functional="0" release="0" software="C:\Github\microsoft-dpe\wa-toolkit-games\code\SocialGames.Cloud\csx\Debug\roles\SocialGames.Web" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="DataConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;SocialGames.Web&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;SocialGames.Web&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;SocialGames.Worker&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.WebInstances" />
            <sCSPolicyFaultDomainMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.WebFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="SocialGames.Worker" generation="1" functional="0" release="0" software="C:\Github\microsoft-dpe\wa-toolkit-games\code\SocialGames.Cloud\csx\Debug\roles\SocialGames.Worker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="DataConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="StatisticsConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;SocialGames.Worker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;SocialGames.Web&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;SocialGames.Worker&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.WorkerInstances" />
            <sCSPolicyFaultDomainMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.WorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="SocialGames.WebFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="SocialGames.WorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="SocialGames.WebInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="SocialGames.WorkerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="79ccf68a-e90f-488a-b7f9-985438401ee7" ref="Microsoft.RedDog.Contract\ServiceContract\SocialGames.CloudContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="5fe02071-ebf5-4838-a8a3-79604a9cc6f4" ref="Microsoft.RedDog.Contract\Interface\SocialGames.Web:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/SocialGames.Cloud/SocialGames.CloudGroup/SocialGames.Web:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>
# NLogFactoryForUnity
A simple NLog logger factory including UnityConsoleTarget.

# Install

Just put `NLogFactory.cs` into your project and configure it by `NLog.config`.


# Dependencies

- .NET Core 2.0+
- NLog 4.6.8
- UnityEngine (DLL included in 2018.4.14f1)

To install NLog from Nuget in Unity, NugetForUnity will be very helpful for you.
https://github.com/GlitchEnzo/NuGetForUnity


# Usage

Typical usage in CSharp scripts is like below;

```csharp
using System.IO;
using UnityEngine;
using NLogFactoryForUnity;

public class Sample : MonoBehaviour
{
    // Declare Logger as a private member
    private NLog.Logger Logger;

    // Start is called before the first frame update
    void Start()
    {
        // Get logger at the start of each modules.
        // If you put `NLog.config` in `Asset/Scripts` you can call `NLogFactory.GetLogger();` without config path.
        var configFilePath = Path.Combine(Application.dataPath, @"NLogFactoryForUnity\sample\Scripts\NLog.config");
        Logger = NLogFactory.GetLogger(configFilePath);

        // Write messages
        Logger.Trace("test", gameObject);
        Logger.Debug("test", gameObject);
        Logger.Info("test", gameObject);
        Logger.Warn("test", gameObject);
        Logger.Error("test", gameObject);
        Logger.Fatal("test", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
```

A sample of `NLog.config` is here.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      throwExceptions="true">

    <extensions>
        <!-- replace this by your assembly name -->
        <add assembly="Assembly-CSharp" />
    </extensions>

    <targets async="true">
        <target name="UCTarget" xsi:type="UnityConsole" />
        <target name="JWSTarget" xsi:type="WebService" protocol="JsonPost" url="http://localhost:5001/nlog">
            <parameter name="SequenceId" layout="${ticks}.${sequenceid:padCharacter=0:padding=6}" />
            <parameter name="LocalTimestamp" layout="${date:format=yyyy-MM-ddTHH\:mm\:ss.fffffff}" />
            <parameter name="Level" layout="${level:uppercase=true}" />
            <parameter name="Callsite" layout="${callsite}" />
            <parameter name="Message" layout="${message}" />
            <parameter name="Error" layout="${exception:format=Message, ToString:separator=\n}" />
        </target>
        
        <!-- Sample for WebServiceTarget for AzureTableService -->
        <!--
        <target name="AZTBLTarget" xsi:type="WebService" protocol="JsonPost" url="YOUR SAS URI with `$format=application/json` parameter">
            <parameter name="PartitionKey" layout="${event-properties:deviceId}" />
            <parameter name="RowKey" layout="${ticks}.${sequenceid:padCharacter=0:padding=6}" />
            <parameter name="LocalTimestamp" layout="${date:format=yyyy-MM-ddTHH\:mm\:ss.fffffff}" />
            <parameter name="Level" layout="${level:uppercase=true}" />
            <parameter name="Callsite" layout="${callsite}" />
            <parameter name="Message" layout="${message}" />
            <parameter name="Error" layout="${exception:format=Message, ToString:separator=\n}" />
        </target>
        -->
    </targets>

    <rules>
        <!-- Disable UCTarget at the production env -->
        <logger name="*" minlevel="Trace" writeTo="UCTarget" />
        <!-- Set proper log level at the production env -->
        <logger name="*" minlevel="Trace" writeTo="JWSTarget" />
        <!--
        <logger name="*" minlevel="Trace" writeTo="AZTBLTarget" />
        -->
    </rules>
</nlog>
```
# VanguardPro

## Quick Start

## How to run
1. Clone this repository.
2. Import database

![image](https://github.com/MQiLEE/AD/assets/95162273/8c6a25f3-b5bc-4b3e-abab-688f65176249)

3. Open web.config file and change the connection string according to your SQL server name. 
```
  <connectionStrings>
	  <add name="db_vanguardpro" connectionString="metadata=res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=LAPTOP-VAAJ4EQ4\SQLEXPRESS;initial catalog=db_vanguardpro;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
  </connectionStrings>
```
change this
```
data source=LAPTOP-VAAJ4EQ4\SQLEXPRESS
```

nuget restore
msbuild CoreBot.sln -p:DeployOnBuild=true -p:PublishProfile=awesomedotnextquestbot-Web-Deploy.pubxml -p:Password=


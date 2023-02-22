# all the packages are extracted to this directory. 
# this folder is typically under d:\Octopus\Work in a folder with DateTime signature
$workingDirectory = $OctopusParameters["env:OctopusCalamariWorkingDirectory"]
$environment = $OctopusParameters["Environment"]

#create output folder for deployment process to place processed template
mkdir output

#remove setting files not needed for the current environment
Get-ChildItem -Path artifacts/configs | Where-Object {$PSItem.Name -ne "appsettings.json" -and $PSItem.Name -ne "appsettings.$($environment).json"} | Remove-Item -Verbose

#list all contents
Get-ChildItem -Path $workingDirectory -Recurse

# Set environment variables
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Write-Host Running on $OctopusParameters["Environment"] environment
Write-Host Setting environment variables

[Environment]::SetEnvironmentVariable("MyGetApiKey",  $OctopusParameters["MyGetApiKey"], "Process")

[Environment]::SetEnvironmentVariable("BranchName", $OctopusParameters["BranchName"] , "Process")
[Environment]::SetEnvironmentVariable("BitBucketToken", $OctopusParameters["BitBucketToken"] , "Process")
[Environment]::SetEnvironmentVariable("octo_api_key", $OctopusParameters["OctoApiKey"] , "Process")

[Environment]::SetEnvironmentVariable("AWS_ACCESS_KEY_ID",  $OctopusParameters["aws-access-key"], "Process")
[Environment]::SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", $OctopusParameters["aws-access-secret"], "Process")
[Environment]::SetEnvironmentVariable("AWS_DEFAULT_REGION", $OctopusParameters["aws-region"], "Process")

[Environment]::SetEnvironmentVariable("Application__LambdaVPCaccess", $OctopusParameters["LambdaVPCaccess"], "Process")
[Environment]::SetEnvironmentVariable("Application__LambdaSecurityGroupIds", $OctopusParameters["LambdaSecurityGroupIds"], "Process")
[Environment]::SetEnvironmentVariable("Application__LambdaVpcSubnetIds", $OctopusParameters["LambdaVpcSubnetIds"], "Process")
[Environment]::SetEnvironmentVariable("Application__Version", $OctopusParameters["Octopus.Release.Number"] , "Process")
[Environment]::SetEnvironmentVariable("Application__AppEnvironment", $OctopusParameters["Environment"] , "Process")
[Environment]::SetEnvironmentVariable("Application__Environment", $OctopusParameters["Environment"] , "Process")
[Environment]::SetEnvironmentVariable("Application__UploaderPrivateKeyString", $OctopusParameters["PrivateKey"] , "Process")
[Environment]::SetEnvironmentVariable("Application__KMSKey", $OctopusParameters["DefaultKMSKeyArn"] , "Process")
[Environment]::SetEnvironmentVariable("Application__UploaderKmsAlias", $OctopusParameters["DefaultKMSKeyAlias"] , "Process")
[Environment]::SetEnvironmentVariable("Application__LambdaTracingMode", $OctopusParameters["LambdaTracingMode"], "Process")
[Environment]::SetEnvironmentVariable("Application__NewRelicApiKey", $OctopusParameters["NewRelicApiKey"], "Process")
[Environment]::SetEnvironmentVariable("Application__ArtifactsS3Bucket", $OctopusParameters["ArtifactsS3BucketName"] , "Process")
[Environment]::SetEnvironmentVariable("Application__ApiGatewayDomainCertificateArn", $OctopusParameters["ApiGatewayDomainCertificateArn"] , "Process")
[Environment]::SetEnvironmentVariable("Application__RoleArn", $OctopusParameters["RoleArn"] , "Process")
[Environment]::SetEnvironmentVariable("Application__Provisioner", "SAM" , "Process")
Write-Host Validate environment vars
$accessKey = [Environment]::GetEnvironmentVariable("AWS_ACCESS_KEY_ID")
$region = [Environment]::GetEnvironmentVariable("AWS_DEFAULT_REGION")
Write-Host AccessKey: $accessKey Region: $region

$hasVpcAccess = [Environment]::GetEnvironmentVariable("Application__LambdaVpcAccess")
$securityGroupIds = [Environment]::GetEnvironmentVariable("Application__LambdaSecurityGroupIds")
$subnetIds = [Environment]::GetEnvironmentVariable("Application__LambdaVpcSubnetIds")

Write-Host VPCAccess: $hasVpcAccess SecurityGroups: $securityGroupIds Subnets: $subnetIds

Write-Host Start deployment
$buildScript = $PSScriptRoot+"\build.ps1"
& $buildScript -t VerifyAndReleaseNext

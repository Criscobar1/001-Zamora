param(
	[string] $environment = "Nunit"
)

Write-Host "Updating $environment database..."
cp SchemaConfig.$environment.xml .\RedCelular.SchemaGenerator\bin\Debug\SchemaConfig.xml
Push-Location .\RedCelular.SchemaGenerator\bin\Debug\
& .\RedCelular.SchemaGenerator.exe -tests
Pop-Location
Write-Host "$environment Database successfully updated!"

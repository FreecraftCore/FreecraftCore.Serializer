##Looks through the entire src directory and runs nuget pack with dependencies added on each csproj found
##foreach file in src/*
foreach($f in Get-ChildItem ./src/)
{
    ##foreach file in the src/*/ directory that ends with the .csproj format
    foreach($ff in (Get-ChildItem (Join-Path ./src/ $f.Name) | Where-Object { $_.Name.EndsWith(".csproj") }))
    {
        ##Add the project path + the csproj name and add the include referenced projects argument which will
        ##force nuget dependencies
        $projectArgs = "pack " + (Join-Path (Join-Path src/ $f.Name) $ff.Name)## + " -IncludeReferencedProjects"
        Start-Process dotnet $projectArgs -Wait
    }
}

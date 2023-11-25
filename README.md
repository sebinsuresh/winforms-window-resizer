WinForms app to ultimately adjust size of windows

## WIP notes

Publishing:
1. "Self contained" - 6 files

```sh
dotnet publish --output ./publish --runtime win-x64 --configuration Release -p:PublishSingleFile=true --self-contained true
```

2. Requires user to have .net 7 runtime
```sh
dotnet publish --output ./publish --runtime win-x64 --configuration Release -p:PublishSingleFile=true --self-contained false
```

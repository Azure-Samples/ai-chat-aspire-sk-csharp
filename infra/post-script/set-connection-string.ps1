param (
	[string]$path
)

Push-Location "$path"

dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:openai" "${OPENAI_CONNECTIONSTRING}"
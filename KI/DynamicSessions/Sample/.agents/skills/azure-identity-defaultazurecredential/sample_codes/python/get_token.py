from azure.identity import DefaultAzureCredential

TENANT_ID = "..."
SCOPE = "https://management.azure.com/.default"


def main() -> None:
    credential = DefaultAzureCredential()
    token = credential.get_token(SCOPE, tenant_id=TENANT_ID)

    print("tenant_id:", TENANT_ID)
    print("scope:", SCOPE)
    print("expires_on:", token.expires_on)
    print("token_prefix:", token.token[:20])


if __name__ == "__main__":
    main()

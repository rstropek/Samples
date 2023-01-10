FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim

# Install .NET 6, jq, SSH server, and Azure Functions Core Tools
RUN wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y \
        azure-functions-core-tools-4 \
        dotnet-sdk-6.0 \
        jq \
        openssh-server 

# Install Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash \
    && az bicep upgrade

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash \
    && apt-get install -y nodejs

# Remove apt cache
RUN rm -rf /var/lib/apt/lists/*

# Setup password for root user
RUN echo 'root:lovetocode' | chpasswd
WORKDIR /root

# Setup SSH
COPY sshd_config /etc/ssh/
COPY entrypoint.sh ./
RUN chmod a+x entrypoint.sh
EXPOSE 22

CMD ["/root/entrypoint.sh"]

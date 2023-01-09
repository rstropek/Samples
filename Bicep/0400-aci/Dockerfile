FROM mcr.microsoft.com/dotnet/sdk:7.0

RUN apt-get update

# Install various tools
RUN apt-get install -y jq openssh-server

# Install Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash

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

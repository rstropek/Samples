# Start with the official Rust image
FROM rust:latest

# Install Node.js LTS and SSH
# Install necessary dependencies
RUN apt-get update && apt-get install -y \
    curl \
    openssh-server \
    && rm -rf /var/lib/apt/lists/*

# Install Node.js LTS using n version manager
RUN curl -fsSL https://raw.githubusercontent.com/tj/n/master/bin/n -o n \
    && bash n lts \
    && rm n \
    && npm install -g npm@latest

# Configure SSH
RUN mkdir /var/run/sshd
RUN echo 'PermitRootLogin yes' >> /etc/ssh/sshd_config
RUN echo 'PasswordAuthentication yes' >> /etc/ssh/sshd_config

# Set root password
RUN echo 'root:LetMeCode123' | chpasswd

# Add cargo bin to PATH in root's .bashrc
RUN echo 'export PATH=$PATH:/usr/local/cargo/bin' >> /root/.bashrc

# Set working directory
WORKDIR /app

# Copy .vscode folder
COPY .vscode/ /app/.vscode/

# Expose SSH port
EXPOSE 22

# Start SSH service and keep container running
CMD ["/usr/sbin/sshd", "-D"]

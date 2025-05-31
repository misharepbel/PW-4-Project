# Use Microsoft's .NET 8 Dev Container image with Docker preinstalled
FROM mcr.microsoft.com/devcontainers/dotnet:0-8.0

# Install Docker CLI and Docker Engine
RUN apt-get update && apt-get install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    gnupg \
    lsb-release \
 && curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg \
 && echo "deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/debian $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null \
 && apt-get update && apt-get install -y docker-ce docker-ce-cli containerd.io \
 && groupadd -f docker \
 && usermod -aG docker vscode


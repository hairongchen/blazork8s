#login
echo $CR_PAT | docker login ghcr.io -u weibaohui --password-stdin

#docker build -t ghcr.io/weibaohui/blazork8s:latest -f BlazorApp/Dockerfile .
#docker push ghcr.io/weibaohui/blazork8s:latest
docker buildx build -t  ghcr.io/weibaohui/blazork8s:0.0.4  -f BlazorApp/Dockerfile   --platform=linux/arm64,linux/amd64 . --push

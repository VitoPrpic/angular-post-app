FROM node:22-alpine AS frontend-build

WORKDIR /src

RUN npm install --global pnpm@11.19.0

COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
RUN pnpm install --frozen-lockfile

COPY angular.json tsconfig.json tsconfig.app.json ./
COPY public ./public
COPY src ./src

RUN pnpm exec ng build


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build

WORKDIR /src

COPY backend/BlogApi/BlogApi.csproj backend/BlogApi/
COPY backend/BlogApi.Application/BlogApi.Application.csproj backend/BlogApi.Application/
COPY backend/BlogApi.Data/BlogApi.Data.csproj backend/BlogApi.Data/
COPY backend/BlogApi.Domain/BlogApi.Domain.csproj backend/BlogApi.Domain/
RUN dotnet restore backend/BlogApi/BlogApi.csproj

COPY backend/ ./backend/
RUN dotnet publish backend/BlogApi/BlogApi.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=backend-build /app/publish ./
COPY --from=frontend-build /src/dist/client ./wwwroot

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "BlogApi.dll"]

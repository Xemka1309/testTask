FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0-jammy AS build

COPY . /source

WORKDIR /source

RUN dotnet publish --use-current-runtime --self-contained false -o /app


FROM mcr.microsoft.com/dotnet/aspnet:6.0-jammy AS final
WORKDIR /app

COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Development
ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid "${UID}" \
    appuser
USER appuser

ENTRYPOINT ["dotnet", "testTask.dll"]

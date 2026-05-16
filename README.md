# The Chopping Block — Private Dev Repository

Private working repository for build preparation, QA, and mobile release of The Chopping Block.

## Project

Unity 6 mobile game (health equity / healthy eating education). Originally built by 404-Found as a Spring 2026 capstone project. This repository handles release engineering, store readiness, and CI builds.

## Build Pipeline

Builds are automated via GitHub Actions. On every push to `main` that touches game assets:

1. **Android APK** — built on Linux runner, downloadable as artifact
2. **iOS IPA (unsigned)** — exported to Xcode project on Linux, packaged to IPA on macOS runner
3. Downloads appear as workflow artifacts in the Actions tab

### Manual trigger

Go to Actions tab → "Build Android APK + iOS IPA" → Run workflow

### Secrets required

| Secret | Purpose |
|--------|---------|
| `UNITY_LICENSE` | Unity .ulf license file contents |
| `UNITY_EMAIL` | Unity account email |
| `UNITY_PASSWORD` | Unity account password |
| `GITEA_TOKEN` | API token for Gitea release uploads |

## Installing builds on test devices

### Android
Transfer APK to phone → enable unknown sources → install

### iOS (iPhone 15 Pro Max)
1. Download unsigned IPA from Actions artifacts
2. Use Sideloadly (Windows) — enter free Apple ID
3. On iPhone: Settings → General → VPN & Device Management → trust your ID
4. App works for 7 days, then re-sign

## Project Structure

- `Assets/` — scenes, scripts, prefabs, sprites, audio, resources
- `Packages/` — Unity package configuration
- `ProjectSettings/` — editor, player, and build settings
- `.github/workflows/build.yml` — CI pipeline
- `Assets/Scripts/Editor/ReleaseBuild.cs` — automated build script

## Build Identity

- Package ID: `com.g2.choppingblock` (Android + iOS)
- Version: `1.0.0`
- Min Android SDK: 24 (Android 7.0)
- Min iOS: 16.0
- Architecture: ARM64 (both platforms)
- Backend: IL2CPP (both platforms)

## Remotes

- `origin` — GitHub (TopTier-Technologies/ChoppingBlock) — CI runs here
- `gitea` — Private Gitea (git.tttmsp.com) — internal mirror, release downloads

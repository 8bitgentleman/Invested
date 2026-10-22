# Building Invested

The project uses **Unity 2020.1.2f1** (Mono scripting backend).

## GitHub Actions (cloud builds)

`.github/workflows/build.yml` builds Windows 64-bit and Linux 64-bit players with
[GameCI](https://game.ci/docs/github/getting-started). It runs on every push, on pull
requests, and manually from **Actions → Build → Run workflow**. Until the license secrets
below are added, it skips the build instead of failing.

> **Cloud builds are missing most models.** 94 of the project's models are `.blend` files,
> and Unity can only import those with Blender installed. GameCI's build images don't have
> Blender, so treat cloud builds as a compile check. Make playable builds locally.

### One-time setup: Unity license secrets

A free Personal license works. Add three repository secrets under
**Settings → Secrets and variables → Actions → New repository secret**:

| Secret           | Value                                         |
|------------------|-----------------------------------------------|
| `UNITY_EMAIL`    | Email of your Unity ID                        |
| `UNITY_PASSWORD` | Password of your Unity ID (no 2FA on this account, or GameCI cannot log in) |
| `UNITY_LICENSE`  | Full contents of your `Unity_lic.ulf` file    |

To get `Unity_lic.ulf` (see GameCI's [activation guide](https://game.ci/docs/github/activation)):

1. Install Unity Hub, sign in, and go to **Preferences → Licenses → Add → Get a free personal license**.
2. Open the license file in a text editor and copy all of it into the `UNITY_LICENSE` secret:
   - Windows: `C:\ProgramData\Unity\Unity_lic.ulf`
   - macOS: `/Library/Application Support/Unity/Unity_lic.ulf`
   - Linux: `~/.local/share/unity3d/Unity/Unity_lic.ulf`

### Downloading a build

Open the workflow run on the **Actions** tab. When it finishes, scroll to **Artifacts** and
download `Invested-StandaloneWindows64` or `Invested-StandaloneLinux64`. Artifacts are kept
for 14 days.

## Building locally

1. Install Unity **2020.1.2f1** through Unity Hub (Installs → Install Editor → Archive),
   with Windows Build Support (Mono).
2. Install [Blender](https://www.blender.org/): Unity needs it to import the `.blend` models.
3. Add the cloned folder in Unity Hub and open it.
4. **File → Build Settings**, pick the platform, then **Build**.

Note: the paid asset pack `Assets/Kevin Iglesias` is not in the repo, so a few animation
clips (Kog, vials, conversation HUD) show up as missing. This only causes warnings.

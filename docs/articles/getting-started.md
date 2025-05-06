# Getting Started

###  1. Fork the repository:

[Visit GitHub repository](https://github.com/DmitriiGoro/Thomason_algorithm_lollipop)

Click "Fork" in top-right corner

### 2. Clone your fork:

```bash
 git clone <url to your fork> \
 cd ThomasonAlgorithm
```
### 3. Build the project:

```bash
 dotnet build src/ThomasonAlgorithm.Core --configuration Release
```
### 4. Reference locally:

- For testing in your projects:
```bash
 dotnet add reference ThomasonAlgorithm/src/ThomasonAlgorithm.Core/ThomasonAlgorithm.Core.csproj
```
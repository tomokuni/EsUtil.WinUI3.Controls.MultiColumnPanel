const { execSync } = require("child_process");
const core = require("@actions/core");
const git = require("simple-git")();

async function run() {
  try {
    const version = core.getInput("version");
    const apiKey = core.getInput("api-key");
    const projects = core.getInput("projects").trim().split("\n");

    // 1: Ensure tag does not exist
    const tag = `v${version}`;
    const tags = await git.tags();
    if (tags.all.includes(tag)) {
      core.info(`Tag ${tag} already exists. Skip publish.`);
      core.setOutput("published-version", "none");
      return;
    }

    // 2: Pack (--no-build)
    execSync(`mkdir -p artifacts`);
    projects.forEach(p => {
      console.log(`Packing ${p} ...`);
      execSync(`dotnet pack "${p}" --no-build -c Release -o artifacts`, { stdio: 'inherit' });
    });

    // 3: Create tag
    await git.addTag(tag);
    await git.pushTags();

    // 4: Create GitHub Release asset upload is handled by GH workflow

    // 5: Publish to NuGet
    execSync(
      `dotnet nuget push artifacts/*.nupkg --api-key ${apiKey} --source https://api.nuget.org/v3/index.json --skip-duplicate`,
      { stdio: 'inherit' }
    );

    core.setOutput("published-version", version);
  }
  catch (err) {
    core.setFailed(err.message);
  }
}

run();
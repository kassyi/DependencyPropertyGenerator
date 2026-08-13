const { execSync } = require('child_process');

try {
  console.log('Installing private agents...');
  execSync('npm install --no-save git+ssh://git@github.com:kassyi/antigravity-global-agents.git#main', { stdio: 'inherit' });
  
  console.log('Running agents...');
  execSync('npx --no-install antigravity-global-agents', { stdio: 'inherit' });
} catch (error) {
  console.error('Failed to update agents:', error);
  process.exit(1);
}

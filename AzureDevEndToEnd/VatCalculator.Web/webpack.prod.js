const { merge } = require('webpack-merge');
const common = require('./webpack.config.js');

module.exports = merge(common, {
    mode: 'production',
    performance: {
        hints: false,
        maxEntrypointSize: 512000,
        maxAssetSize: 512000
    },
    module: {
        rules: [
            {
                test: /index\.ts$/,
                loader: 'string-replace-loader',
                options: {
                    search: '__SERVICE_URL__',
                    replace: 'https://app-7rrdjpf3fi46g.azurewebsites.net',
                }
            },
        ],
    }
});

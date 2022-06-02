const HTMLWebpackPlugin = require('html-webpack-plugin');
const path = require('path');

module.exports = {
  entry: "./src/index.ts",
  experiments: {
    syncWebAssembly: true
  },
  module: {
    rules: [
      {
        test: /index\.ts$/,
        loader: 'string-replace-loader',
        options: {
          search: '__SERVICE_URL__',
          replace: 'http://localhost:8081',
        }
      },
      {
        test: /index\.ts$/,
        loader: 'string-replace-loader',
        options: {
          search: '__APPINSIGHTS_CONNECTION_STRING__',
          replace: 'InstrumentationKey=b569e6d2-56c1-4c9a-9455-0ad9e1892bdb;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/',
        }
      },
      {
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/,
      },
      {
        test: /\.css$/i,
        use: ["style-loader", "css-loader"],
      }
    ],
  },
  resolve: {
    extensions: ['.tsx', '.ts', '.js'],
  },
  output: {
    path: path.resolve(__dirname, "dist"),
    filename: "bundle.js",
  },
  mode: "development",
  plugins: [
    new HTMLWebpackPlugin({
      template: path.resolve(__dirname, 'src/index.html'),
    }),
  ]
};

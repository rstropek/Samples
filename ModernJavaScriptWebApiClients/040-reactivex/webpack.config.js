var HtmlWebpackPlugin = require('html-webpack-plugin');
module.exports = {
  entry: './index.ts',
  output: {
    filename: 'bundle.js',
    path: __dirname + '/dist'
  },
  resolve: {
    extensions: ['.webpack.js', '.web.js', '.ts', '.tsx', '.js']
  },
  module: {
    loaders: [
      { test: /\.tsx?$/, loader: 'ts-loader' }
    ]
  },
  plugins: [new HtmlWebpackPlugin({
    title: 'RxJS Intro',
    filename: 'index.html'
  })]
}

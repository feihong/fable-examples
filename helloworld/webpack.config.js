const path = require('path')
const webpack = require('webpack')

const cfg = {
  devtool: 'source-map',
  entry: './out/helloworld/hello.js',
  output: {
    path: path.join(__dirname, 'public'),
    filename: 'bundle.js'
  },
  module: {
    preLoaders: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        loader: 'source-map-loader'
      }
    ]
  }
}

module.exports = cfg

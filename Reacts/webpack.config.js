const path = require('path');
const webpack = require('webpack');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = (env, argv) =>
{
    const isProduction = argv.mode === 'production';
    const reactorPath = isProduction ? 'https://kusmeeks.net/reactor' : '/reactor';
    const configuredCachePath = 'https://kusdepot-reacts.azureedge.net/';
    const cachePath = isProduction ? configuredCachePath : '/';
    return {
        mode: isProduction ? 'production' : 'development',
        entry: './Client/StartUp.tsx',
        devtool: isProduction ? 'source-map' : 'eval-source-map',
        module: {
            rules: [
                {
                    test: /\.tsx?$/,
                    use: 'ts-loader',
                    exclude: /node_modules/,
                },
                {
                    enforce: 'pre',
                    test: /\.js$/,
                    use: ['source-map-loader'],
                },
                {
                    test: /\.(webp)$/i,
                    type: 'asset/resource',
                    generator: {
                        filename: '[name][ext]'
                    }
                },
                {
                    test: /\.css$/i,
                    use: [isProduction ? MiniCssExtractPlugin.loader : 'style-loader', 'css-loader'],
                }
            ],
        },
        resolve: {
            extensions: ['.tsx', '.ts', '.js'],
        },
        output: {
            filename: 'kusdepot.js',
            path: path.resolve(__dirname, 'wwwroot'),
            clean: true,
            publicPath: cachePath,
        },
        optimization: {
            minimize: isProduction,
        },
        plugins: [
            new CopyWebpackPlugin({
                patterns: [
                    { from: 'Client/favicon.ico', to: 'favicon.ico' },
                    { from: 'Client/Client.html', to: 'Client.html' },
                ]
            }),
            new webpack.DefinePlugin({
                REACTOR_PATH: JSON.stringify(reactorPath)
            }),
            new MiniCssExtractPlugin({
                filename: 'kusdepot.css'
            })
        ].filter(Boolean),
    };
};

const path = require('path');
const webpack = require('webpack');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = (env, argv) =>
{
    const isProduction = argv.mode === 'production';
    const configuredCachePath = 'https://kusdepot-solutions.azureedge.net/';
    const cachePath = isProduction ? configuredCachePath : '/';
    return {
        mode: isProduction ? 'production' : 'development',
        entry: './Client/StartUp.ts',
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
                    test: /\.(webp|png|jpg|svg)$/i,
                    type: 'asset/resource',
                    generator: {
                        filename: '[name][ext]'
                    }
                },
                {
                    test: /\.(woff2?|ttf|otf|eot)$/i,
                    type: 'asset/resource',
                    generator: {
                        filename: 'fonts/[name][ext]',
                        publicPath: '/'
                    }
                },
                {
                    test: /\.css$/i,
                    use: [MiniCssExtractPlugin.loader, 'css-loader'],
                }
            ],
        },
        resolve: {
            extensions: ['.tsx', '.ts', '.js'],
        },
        output: {
            filename: 'kusdepotsolutions.js',
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
                    { from: 'Client/index.html', to: 'index.html' },
                    { from: 'Client/icononly.png', to: 'icononly.png' },
                    { from: 'Client/logo.png', to: 'logo.png' },
                    { from: 'Client/preview.png', to: 'preview.png' },
                    { from: 'Client/favicon.ico', to: 'favicon.ico' },
                    { from: 'Client/robots.txt', to: 'robots.txt' },
                    { from: 'Client/sitemap.xml', to: 'sitemap.xml' },
                ]
            }),
            new MiniCssExtractPlugin({
                filename: 'kusdepotsolutions.css'
            })
        ].filter(Boolean),
    };
};

const path = require('path');

module.exports = (env, argv) => {
    return {
    entry: {
        partyapp: './ClientApp/src/index.js'
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: ['babel-loader']
            },
            {
                test: /\.(png|jpe?g|gif|svg)$/i,
                use: [
                    {
                        loader: 'file-loader',
                    },
                ],
            }, {
                test: /\.css$/i,
                use: ['style-loader', 'css-loader']
            }
        ]
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
        }, optimization: {
        minimize: argv.mode == 'production' ? true : false
    },
    output: {
        filename: '[name].bundle.js',
            path: path.resolve(__dirname, 'wwwroot/buildoutput/')
    },
    watchOptions: {
        poll: true,
            ignored: /node_modules/
    },
    devtool: 'source-map'
}  
};

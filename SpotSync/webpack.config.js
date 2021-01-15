const path = require('path');

module.exports = {
    entry: {
        main: './TypeScript/index.ts',
        party: './Views/Party/index.ts'
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },
    output: {
        filename: '[name].bundle.js',
        library: 'Spotibro',
        path: path.resolve(__dirname, 'wwwroot/js')
    },
};
/// <binding BeforeBuild='build' ProjectOpened='build' />
const { parallel, src, dest } = require('gulp'),
    sass = require('gulp-sass')
var baseFilepath = "./wwwroot/";

function buildSass() {
    return src(baseFilepath + 'scss/style.scss')
        .pipe(sass())
        .pipe(dest(baseFilepath + 'css'));
}

exports.build = parallel(buildSass);
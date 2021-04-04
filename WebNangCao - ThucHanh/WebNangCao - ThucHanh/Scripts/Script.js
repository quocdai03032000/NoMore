function ChangeImage(UploadImage, previewImg) {
    if (UploadImage.file && UploadImage.file[0]) {
        var render = new FileReader();
        reader.onload = function (e) {
            $(previewImg).attr('src', e.target.result);
        }
        reader.readAsDataURL(UploadImage.file[0]);
    }
}
function ShowImg(imageUploader, previewImg) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImg).attr('src', e.target.result);
        }
        // doc duong dan cua du lieu
        reader.readAsDataURL(imageUploader.files[0]);
    }

}
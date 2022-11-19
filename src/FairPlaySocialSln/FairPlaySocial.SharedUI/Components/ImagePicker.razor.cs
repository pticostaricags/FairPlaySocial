using FairPlaySocial.Models.Photo;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FairPlaySocial.SharedUI.Components
{
    public partial class ImagePicker
    {
        [Parameter]
        [EditorRequired]
        public CreatePhotoModel? Model { get; set; }
        [Parameter]
        [EditorRequired]
        public EventCallback OnFileSelected { get; set; }
        [Parameter]
        [EditorRequired]
        public int MaxHeightForPreview { get; set; }

        private async Task OnFileSelectionChangedAsync(InputFileChangeEventArgs inputFileChangeEventArgs)
        {
            if (inputFileChangeEventArgs.FileCount == 1)
            {
                //TODO: Invoke Content Moderation endpoints to reject prohibited images
                //TODO: Invoke Face/Computer Vision APIs to reject images without a face
                int allowedMegaBytes = 10;
                int allowedBytes = allowedMegaBytes * 1024 * 1024;
                using var fileStream = inputFileChangeEventArgs.File.OpenReadStream(maxAllowedSize: allowedBytes, base.CancellationToken);
                using MemoryStream memoryStream = new();
                await fileStream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                this.Model!.Filename = inputFileChangeEventArgs.File.Name;
                this.Model!.ImageBytes = fileBytes;
                this.Model!.ImageType = inputFileChangeEventArgs.File.ContentType;
                await this.OnFileSelected.InvokeAsync();
            };
        }
    }
}

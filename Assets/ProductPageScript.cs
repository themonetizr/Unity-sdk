using Assets.Monetizr.Dto;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProductPageScript : MonoBehaviour
{
    public Image BigImage;
    public HorizontalLayoutGroup ImageButtons;
    public Text HeaderText;
    public Text PriceText;
    public Button CheckoutButton;
    public Image PicturesButton;


    public void Init(ProductInfo info)
    {
        var firstVariant = info.data.productByHandle.variants.edges.FirstOrDefault();
        PriceText.text = $"{firstVariant.node.priceV2.currencyCode} {firstVariant.node.priceV2.amount}";
        HeaderText.text = info.data.productByHandle.title;
        InitImages(info.data.productByHandle.images);
    }

    private void InitImages(Images images)
    {
        if (images?.edges?.FirstOrDefault()?.node?.transformedSrc == null)
            return;

        StartCoroutine(isDownloading(images.edges.FirstOrDefault().node.transformedSrc, BigImage));
        foreach (var img in images.edges)
        {
            var uiImage = Instantiate(PicturesButton, ImageButtons.transform, false);
            var button = uiImage.GetComponent<Button>();
            button.onClick.AddListener(() => { BigImage.sprite = uiImage.sprite; });
            StartCoroutine(isDownloading(img.node.transformedSrc, uiImage));
        }
    }

    IEnumerator isDownloading(string url, Image targetImage)
    {
        // Start a download of the given URL
        var www = new WWW(url);
        // wait until the download is done
        yield return www;
        // Create a texture in DXT1 format
        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);

        // assign the downloaded image to sprite
        www.LoadImageIntoTexture(texture);
        Rect rec = new Rect(0, 0, texture.width, texture.height);
        Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
        targetImage.sprite = spriteToUse;

        www.Dispose();
        www = null;
    }
}

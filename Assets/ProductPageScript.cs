using Assets.Monetizr.Dto;
using Assets.SingletonPattern;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
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
    public Button CloseButton;
    public Image PicturesButton;
    public InAppBrowserBridge Bridge;
    public List<VariantsDropdown> Dropdowns;
    private ProductInfo _productInfo;
    private string _tag;
    Dictionary<string, List<string>> _productOptions;

    public void Init(ProductInfo info, string tag)
    {
        _productOptions = new Dictionary<string, List<string>>();
        var options = info.data.productByHandle.variants.edges.SelectMany(x => x.node.selectedOptions.Select(y => y.name)).Distinct().ToList();
        int i = 0;
        foreach (var dd in Dropdowns)
        {
            dd.gameObject.SetActive(false);
        }

        foreach (var option in options)
        {
            var possibleOptions = info.data.productByHandle.variants.edges.SelectMany(x => x.node.selectedOptions.Select(y => y)).Where(x => x.name == option).Select(x => x.value).Distinct().ToList();
            _productOptions.Add(option, possibleOptions);

            if (i < 5)
            {
                var dd = Dropdowns.ElementAt(i);
                dd.Init(possibleOptions, option);
                dd.gameObject.SetActive(true);
                i++;
            }
        }
        _productInfo = info;
        _tag = tag;
        var firstVariant = info.data.productByHandle.variants.edges.FirstOrDefault();
        PriceText.text = $"{firstVariant.node.priceV2.currencyCode} {firstVariant.node.priceV2.amount}";
        HeaderText.text = info.data.productByHandle.title;
        InitImages(info.data.productByHandle.images);
        CheckoutButton.onClick.AddListener(() => { OpenShop(); });
        CloseButton.onClick.AddListener(() => { CloseProductPage(); });
    }

    private void CloseProductPage()
    {
        MonetizrClient.Instance.RegisterProductPageDismissed(_tag);
        Destroy(gameObject);
    }

    private void OpenShop()
    {
        Bridge.onBrowserClosed.AddListener(() => { });

        VariantsEdge selectedEdge = null;
        foreach (var variant in _productInfo.data.productByHandle.variants.edges)
        {
            bool rightOption = true;
            foreach (var dd in Dropdowns)
            {
                if (!dd.isActiveAndEnabled)
                    continue;

                var option = variant.node.selectedOptions.FirstOrDefault(x => x.name == dd.OptionName);
                if (option.value != dd.SelectedOption)
                    rightOption = false;
            }

            if (rightOption)
                selectedEdge = variant;

            if (selectedEdge != null)
                break;
        }

        var url = _productInfo.data.productByHandle.onlineStoreUrl;
        if (selectedEdge?.node?.id != null)
        {
            var request = new VariantStoreObject()
            {
                product_handle = _tag,
                quantity = 1,
                variantId = selectedEdge.node.id
            };
            var jsonData = JsonConvert.SerializeObject(request);
            var response = MonetizrClient.Instance.PostDataWithResponse("products/checkout", jsonData);
            if (response != null)
            {
                var checkoutObject = JsonConvert.DeserializeObject<CheckoutResponse>(response);
                if(checkoutObject.data.checkoutCreate.checkoutUserErrors==null || !checkoutObject.data.checkoutCreate.checkoutUserErrors.Any())
                    url = checkoutObject.data.checkoutCreate.checkout.webUrl;
            }
        }

        
        InAppBrowser.OpenURL(url);
        MonetizrClient.Instance.RegisterClick();
        Destroy(gameObject);
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

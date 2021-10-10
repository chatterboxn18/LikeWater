using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LikeWater
{
    

public class LWStreamController : MonoBehaviour
{
    [SerializeField] private Transform _appContainer; 
    [SerializeField] private Transform _videoContainer;

    [SerializeField] private LWMediaCard _videoPrefab;
    [SerializeField] private LWMediaItem _streamPrefab;

    private void Start()
    {
        var streamItem = LWResourceManager.StreamItem;
        /*foreach (var app in streamItem.AppLinks)
        {
            var appLink = Instantiate(_streamPrefab, _appContainer);
            appLink.SetItem(true, app.Name, app.Link);
        }*/

        foreach (var video in streamItem.Videos)
        {
            var card = Instantiate(_videoPrefab, _videoContainer);
            StartCoroutine(card.SetMediaCard(video));
        }
    }
}
}

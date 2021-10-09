using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace LikeWater
{
	public class LWCardManager : MonoBehaviour
	{
		
		private string _startDate;

		private string _cardJSON;
		private Dictionary<string, Sprite> _spriteSheets = new Dictionary<string, Sprite>();
		private FileService _fileService;
		
		private static Dictionary<string, Dictionary<int, Sprite>> _cardDictionary = new Dictionary<string, Dictionary<int,Sprite>>();

		public static Dictionary<string, Dictionary<int, Sprite>> CardDictionary => _cardDictionary;
		
		private IEnumerator Start()
		{
			while (!LWResourceManager.IsLoaded)
				yield return null;
			_fileService = (FileService) ServiceManager.ServiceCollection[ServiceManager.Services.FileService];
			yield return Setup();
		}

		private IEnumerator Setup()
		{
			if (string.IsNullOrEmpty(_cardJSON))
			{
				yield return _fileService.GetFile("likewater-cards.json", (succeed, handler) =>
				{
					if (succeed)
					{
						_cardJSON = handler.text;
					}
				});
			}

			if (string.IsNullOrEmpty(_cardJSON))
			{
				Debug.LogError("The data is empty");
			}
			
			var flowers = LWResourceManager.Flowers;

			foreach (var flower in flowers)
			{
				yield return LoadCard(flower.Index);
			}
		}

		public void UnlockCard(int index)
		{
			var data = JSON.Parse(_cardJSON);
			string sheet = data[index.ToString()]["sheet-name"];
			if (LWCardData.current.CollectedCards.ContainsKey(sheet))
			{
				LWCardData.current.CollectedCards[sheet][index].AmountCollected++;
				SerializationManager.Save(LWConfig.CardDataSaveName, LWCardData.current);
			}
			else
			{
				Debug.LogError("Key " + sheet + "doesn't exist");
			}
		}

		public Sprite GetCardSprite(int index)
		{
			var data = JSON.Parse(_cardJSON);
			string sheet = data[index.ToString()]["sheet-name"];
			return _cardDictionary[sheet][index];
		}
		
		private IEnumerator LoadCard(int index)
		{
			var data = JSON.Parse(_cardJSON);
			string sheet = data[index.ToString()]["sheet-name"];
			if (!_spriteSheets.ContainsKey(sheet))
			{
				yield return _fileService.GetTexture(sheet, (success, handler) =>
				{
					if (success)
					{
						var texture = (DownloadHandlerTexture) handler;
						_spriteSheets.Add(sheet, Extensions.Texture2DToSprite(texture.texture));
					}
				});
			}

			var cardData = data[index.ToString()];
			var indexString = cardData["sheet-index"];
			var collectTotal = cardData["unlock-amount"];
			var member = cardData["member"];
			var indexs = indexString.Value.Split(',');
			var x = int.Parse(indexs[0]);
			var y = int.Parse(indexs[1]);
			var cardSprite = Sprite.Create(_spriteSheets[sheet].texture,
				new Rect(LWConfig.CardWidth * x, LWConfig.CardHeight * y, LWConfig.CardWidth, LWConfig.CardHeight),
				new Vector2(0.5f, 0.5f));
			if (!_cardDictionary.ContainsKey(sheet))
			{
				_cardDictionary.Add(sheet, new Dictionary<int, Sprite>());
			}
			_cardDictionary[sheet].Add(index, cardSprite);

			// Adding to the Data set in save file
			if (!LWCardData.current.CollectedCards.ContainsKey(sheet))
			{
				LWCardData.current.CollectedCards.Add(sheet, new Dictionary<int, LWCardData.FlowerCard>());
			}

			if (LWCardData.current.CollectedCards[sheet].ContainsKey(index))
			{
				if (index < LWCardData.current.CollectedCards[sheet].Count)
				{
					yield break;
				}
			}
			var newCard = new LWCardData.FlowerCard();
			newCard.Member = member;
			newCard.X = x;
			newCard.Y = y;
			newCard.PlantIndex = index;
			newCard.CollectTotal = collectTotal;
			newCard.AmountCollected = 0;
			LWCardData.current.CollectedCards[sheet].Add(index, newCard);

			SerializationManager.Save(LWConfig.CardDataSaveName, LWCardData.current);

			yield return null;
		}
	}
}
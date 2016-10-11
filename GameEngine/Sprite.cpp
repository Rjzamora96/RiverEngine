#include "Sprite.h"

namespace RiverEngine
{
	typedef void(*AddFunc)(Sprite*, std::string);
	typedef void(*ChangeSprite)(Sprite*, std::string);
	typedef void(*AddTexture)(std::string);

	AddFunc Sprite::addSprite;
	ChangeSprite Sprite::changeSprite;
	AddTexture Sprite::addTexture;
}
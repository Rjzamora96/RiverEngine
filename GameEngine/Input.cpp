#include "Input.h"

namespace RiverEngine
{
	std::unordered_map<std::string, Input::Keys> Input::keyMap;
	BindPtr Input::keyDown;
	BindPtr Input::keyPressed;
	BindPtr Input::keyReleased;

	void Input::InitializeBindings()
	{
		keyMap["Up"] = Keys::Up;
		keyMap["Down"] = Keys::Down;
		keyMap["Left"] = Keys::Left;
		keyMap["Right"] = Keys::Right;
		keyMap["W"] = Keys::W;
		keyMap["S"] = Keys::S;
		keyMap["A"] = Keys::A;
		keyMap["D"] = Keys::D;
		keyMap["Space"] = Keys::Space;
	}

	bool Input::IsKeyDown(std::string key)
	{
		return keyDown(keyMap[key]);
	}

	bool Input::IsKeyPressed(std::string key)
	{
		return keyPressed(keyMap[key]);
	}

	bool Input::IsKeyReleased(std::string key)
	{
		return keyReleased(keyMap[key]);
	}
}
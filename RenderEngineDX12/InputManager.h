#pragma once

#include "Keyboard.h"

class InputManager
{
public:
	static void Initialize();
	static bool CheckKeyDown(int key);
	static bool CheckKeyPressed(int key);
	static bool CheckKeyReleased(int key);
	static void Update();
private:
	static std::unique_ptr<DirectX::Keyboard> m_keyboard;
	static DirectX::Keyboard::KeyboardStateTracker m_tracker;
};


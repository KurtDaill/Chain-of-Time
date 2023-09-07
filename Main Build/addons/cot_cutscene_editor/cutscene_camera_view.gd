@tool
extends TextureRect

func on_new_camera_enabled(camera : Camera3D):
    texture = camera.get_texture()


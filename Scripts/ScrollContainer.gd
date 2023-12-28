tool
extends ScrollContainer
## This script is used to add a little space at the left of the 
## scroll container.
##
## By default the scroll container add a scroll bar when there is not 
## enough space to render all element that it contains. The problem is that
## is move the horizontal alligment of element inside.
## This script just create the space for the scroll bar before the scrollbar
## appear so it doesn't change the alligment of element inside when it appear.

onready var _v_scroll := get_v_scrollbar()
onready var margin := $MarginContainer

func _ready() -> void:
	OS.min_window_size = Vector2(200, 200)
	var _e = _v_scroll.connect("visibility_changed", self, "_on_scroll_bar_visibility_changed")

func _on_scroll_bar_visibility_changed() -> void:
	if _v_scroll.visible:
		margin.set("custom_constants/margin_right", 0)
	else:
		margin.set("custom_constants/margin_right", _v_scroll.rect_size.x)

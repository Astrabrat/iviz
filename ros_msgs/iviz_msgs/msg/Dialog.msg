uint8 ACTION_ADD = 0
uint8 ACTION_REMOVE = 1
uint8 ACTION_REMOVEALL = 2

uint8 TYPE_PLAIN  = 0  # a dialog with title, text, and a row of buttons 
uint8 TYPE_SHORT  = 1  # a smaller dialog with title and text
uint8 TYPE_NOTICE = 2  # a dialog with an icon and a text
uint8 TYPE_MENU   = 3  # a dialog with a menu of multiple options
uint8 TYPE_BUTTON = 4  # a tiny clickable dialog
uint8 TYPE_ICON   = 5  # a dialog with title, text, icon, and a row of buttons

uint8 BUTTONS_OK              = 0
uint8 BUTTONS_YESNO           = 1
uint8 BUTTONS_OKCANCEL        = 2
uint8 BUTTONS_FORWARD         = 3
uint8 BUTTONS_FORWARDBACKWARD = 4
uint8 BUTTONS_BACKWARD        = 5

uint8 ICON_NONE     = 0
uint8 ICON_CROSS    = 1
uint8 ICON_OK       = 2
uint8 ICON_FORWARD  = 3
uint8 ICON_BACKWARD = 4
uint8 ICON_DIALOG   = 5
uint8 ICON_UP       = 6
uint8 ICON_DOWN     = 7
uint8 ICON_INFO     = 8
uint8 ICON_WARN     = 9
uint8 ICON_ERROR    = 10
uint8 ICON_DIALOGS  = 11
uint8 ICON_QUESTION = 12

uint16 ALIGNMENT_DEFAULT        = 0
uint16 ALIGNMENT_LEFT           = 1
uint16 ALIGNMENT_CENTER         = 2
uint16 ALIGNMENT_RIGHT          = 4
uint16 ALIGNMENT_JUSTIFIED      = 8
uint16 ALIGNMENT_FLUSH          = 16
uint16 ALIGNMENT_GEOMETRYCENTER = 32
uint16 ALIGNMENT_TOP            = 256
uint16 ALIGNMENT_MID            = 512
uint16 ALIGNMENT_BOTTOM         = 1024

std_msgs/Header header
byte action
string id
duration lifetime  # if not zero, the dialog will be removed after this time
float64 scale
byte type
byte buttons
byte icon
std_msgs/ColorRGBA background_color
string title
string caption
uint16 caption_alignment
string[] menu_entries
byte binding_type
geometry_msgs/Vector3 tf_offset
geometry_msgs/Vector3 dialog_displacement
geometry_msgs/Vector3 tf_displacement


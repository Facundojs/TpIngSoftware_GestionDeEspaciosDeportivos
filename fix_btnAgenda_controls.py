import re

path = "TpIngSoftware_GestionDeEspaciosDeportivos/FrmEspacios.Designer.cs"

with open(path, "r") as f:
    content = f.read()

# I see it wasn't added to panelEditor.Controls.Add(this.btnAgenda) properly because I matched `this.Controls.Add(this.btnLimpiar)` which probably didn't exist or something else.
# Let's fix it.

if "this.panelEditor.Controls.Add(this.btnAgenda);" not in content:
    content = content.replace("this.panelEditor.Controls.Add(this.btnLimpiar);", "this.panelEditor.Controls.Add(this.btnLimpiar);\n            this.panelEditor.Controls.Add(this.btnAgenda);")

# Also the location was 400, 310, let's put it next to Limpiar or somewhere visible inside panelEditor
# Limpiar is at 23, 280. We can put btnAgenda at 23, 310.
content = content.replace("this.btnAgenda.Location = new System.Drawing.Point(400, 310);", "this.btnAgenda.Location = new System.Drawing.Point(23, 310);")

with open(path, "w") as f:
    f.write(content)

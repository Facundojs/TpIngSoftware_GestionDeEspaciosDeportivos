import re

path = "TpIngSoftware_GestionDeEspaciosDeportivos/FrmEspacios.Designer.cs"
with open(path, "r") as f:
    content = f.read()

# Let's insert the definition of btnAgenda right after btnLimpiar definition
btn_agenda_def = """            //
            // btnAgenda
            //
            this.btnAgenda.Location = new System.Drawing.Point(23, 310);
            this.btnAgenda.Name = "btnAgenda";
            this.btnAgenda.Size = new System.Drawing.Size(250, 23);
            this.btnAgenda.TabIndex = 10;
            this.btnAgenda.Text = "Configurar Agenda";
            this.btnAgenda.UseVisualStyleBackColor = true;
            this.btnAgenda.Click += new System.EventHandler(this.btnAgenda_Click);
"""

# check if it already exists somewhere
if "this.btnAgenda.Location =" in content:
    # remove old if any
    pass
else:
    content = content.replace("            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);", "            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);\n" + btn_agenda_def)

with open(path, "w") as f:
    f.write(content)

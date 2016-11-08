import xml.etree.ElementTree as ET
reseau = ET.parse("plan5x5.xml").getroot()
ID = 1
for child in reseau:
    if child.tag == "noeud":
        print("pointsAttendus.Add({0}, new Point({0}, {1}, {2}));".format(
              child.attrib['id'], child.attrib['x'], child.attrib['y']))
    else:
        xpath_orig = './/noeud[@id="{}"]'.format(child.attrib['origine'])
        noeud_orig = reseau.find(xpath_orig)
        xpath_dest = './/noeud[@id="{}"]'.format(child.attrib['destination'])
        noeud_dest = reseau.find(xpath_dest)
        print("""tronconsAttendus.Add({0}, new Troncon(new Point({1}, {2}, {3}),
                                     {4},
                                     new Point({5}, {6}, {7}),
                                     {8}, "{9}", {0}));
              """.format(
              ID, noeud_dest.attrib['id'], noeud_dest.attrib['x'],
              noeud_dest.attrib['y'], child.attrib['longueur'],
              noeud_orig.attrib['id'], noeud_orig.attrib['x'],
              noeud_orig.attrib['y'], child.attrib['vitesse'],
              child.attrib['nomRue']))
        ID += 1

X = [int(noeud.attrib['x']) for noeud in reseau.findall('.//noeud')]
Y = [int(noeud.attrib['y']) for noeud in reseau.findall('.//noeud')]
print("structAttendue.Xmax = {};".format(max(X)))
print("structAttendue.Xmin = {};".format(min(X)))
print("structAttendue.Ymax = {};".format(max(Y)))
print("structAttendue.Ymin = {};".format(min(Y)))

using System.Data.Entity;
using System.Linq;

namespace BeeInMyGarden.Data
{
	public class BlogContext : DbContext
	{
		public BlogContext()
			: base("AlmDays")
		{
		}

		public DbSet<BlogItem> BlogItems { get; set; }

		/// <summary>
		/// Generates demo data
		/// </summary>
		/// <remarks>
		/// This demo data is taken from the blog http://bienenimgarten.wordpress.com
		/// </remarks>
		public void GenerateDemoData()
		{
			var existingBlogIds = this.BlogItems.Select(b => b.BlogId).ToArray();

			if (!existingBlogIds.Contains(3))
			{
				var newItem = new BlogItem()
				{
					BlogId = 3,
					Title = "Der erste, selbst gefangene Bienenschwarm",
					FeaturedImageUri = "http://bienenimgarten.files.wordpress.com/2013/06/img_1509.jpg?w=500",
					Content = @"
Der Anruf
---------

Gestern war ein ereignisreicher Tag in unserem jungen Imkerleben. Als wir am Abend von einem Ausflug mit dem Moutainbike nach 
Hause gekommen sind, erreichte uns ein Anruf vom Obmann unseres Imkervereins. Er wurde verständigt und um Hilfe gebeten, da 
sich ganz in unserer Nähe ein Bienenscharm in einem privaten Garten niedergelassen hatte. Er fragte uns, um wir uns darum 
kümmern möchten. Wow, das war eine unvorhergesehene Herausforderung. Wir hatten erst bei einem Schwarm – unserem eigenen – geholfen 
und jetzt sollten wir uns gleich alleine um das Einfangen kümmern. Aber was soll’s, Erfahrung macht den Meister. Also haben 
wir zugesagt und uns bei dem betroffenen Gartenbesitzer telefonisch angekündigt.

Für alle, die auch mal in die Situation kommen, ihren ersten Schwarm einfangen zu müssen, hier eine Checkliste mit Sachen, 
die sich als nützlich erwiesen:

* Ein großer Sack zum Einfangen des Schwarms (wir mussten einen Plastiksack nehmen; ein großer Stoffsack wäre wahrscheinlich noch viel besser)
* Eine Beute, in die man den Schwarm einquartieren kann (als “Beute” bezeichnet man die Behausung der Bienen)
* Spanngurte, um die Beute für den Transport fixieren zu können.
* Astschere bzw. Säge, um kleine Zweige, die beim Einfangen im Baum stören, entfernen zu können
* Leiter
* Taschenlampe (sehr praktisch, wenn man den Schwarm am späten Abend abtransportieren möchte)
* Natürlich die üblichen Imker-Utensilien wie Besen und Stockmeisel
* Entsprechende Bekleidung (ein Schwarm ist nach unserer Erfahrung und laut Literatur sehr friedlich; trotzdem fühlen wir uns mit Imkeranzug und Handschuhen deutlich sicherer, wenn mehrere zehntausend Bienen um einen herumschwirren)

Die Ankunft
-----------

Die nächste Überraschung wartete auf uns, als wir beim betroffenen Garten ankamen. Dort saß eine große Familie im Garten bei 
Kaffee und Kuchen und in ca. 8(!) Metern Höhe über ihnen im Kirschbaum der Bienenschwarm. Das gab uns erstmal zu denken.

Der Kirschbaum steht nicht auf dem Grundstück der Familie, die uns angerufen hat. Nur die Äste des Baumes reichten bis auf 
deren Grund. Wir gingen also als erstes zum Nachbarn, dem der Baum gehört. Wir hatten Glück und wurden auch dort äußerst nett 
und zuvorkommend begrüßt. Generell stellten wir wieder einmal fest, dass Bienen echte Sympathieträger sind. Keine Rede davon, 
dass sie als Belästigung oder gar Gefahr wahrgenommen wurden.

Am Nachbargrundstück sahen wir, dass dort die Situation nur ein wenig besser war. Der Baum steht auf einem Hang und deshalb 
reduzierte sich die zu überwindende Höhe zum Schwarm auf 6 Meter. Immer noch ganz ordentlich. Was müssen sich die Bienen aber 
auch ausgerechnet ganz oben in die Krone setzen. Der Hausbesitzer bot uns eine entsprechend hohe Leiter an. Mit der konnten 
wir – zwar etwas wacklig aber doch – in die Baumkrone steigen und uns den Schwarm aus der Nähe ansehen.

![](http://bienenimgarten.files.wordpress.com/2013/06/img_1508.jpg?w=960)

Das Einfangen
-------------

Bis auf die unangenehm hohe Position des Schwarms war das Einfangen an sich relativ problemlos. Ein paar störende, kleine Äste 
vom Baum geschnitten, einen großen Sack unter den Schwarm gehalten, kräftig am Ast gerüttelt und schon hatten wir den größten 
Teil des Schwarms sprichwörtlich “im Sack”. Wir achteten darauf, dass jetzt alles schnell ging. Runter von der Leiter und sofort 
in die Beute geleert. Die Bienen, die nicht aus dem Sack kommen wollten, ließen wir drauf sitzen und gaben ihnen Zeit, ihren 
Kollegen zu folgen.

![](http://bienenimgarten.files.wordpress.com/2013/06/img_1512.jpg?w=960)

Als wir nach einer Stunde wieder kamen, waren alle Bienen, die wir vom Baum geholt hatten, in der Beute. Wir stellten allerdings 
fest, dass wir ca. ein Viertel der Bienen durch die schlechte Zugänglichkeit am Baum nicht erwischt hatten. Sie hingen noch in 
der Baumkrone. Also nochmals in den Imkeranzug, rauf auf den Baum und die restlichen eingesammelt. Beim zweiten Mal ging das alles 
relativ unkompliziert und schnell.

Der Transport und die Ankunft
-----------------------------

Es ist immer wieder ein interessantes Gefühl, mit über 20.000 brummenden Bienen im Auto herumzufahren. Im Grunde verlief der 
Transport aber problemlos. Wir konnten sie noch gestern Nacht an ihren Standplatz bringen. Jetzt haben wir also zwei Völker, 
um die wir uns kümmern dürfen.

Heute Früh trat noch ein spannendes Phänomen auf: Ein Teil der Bienen setzte sich außen auf die Beute (siehe Foto unten). 
In Foren und von erfahrenen Imkerkollegen haben wir erfahren, dass das gut sein kann. Das machen die Bienen zur 
Temperaturregulierung und wenn sie keinen Platz im Stock haben. Wie schon berichtet, imkern wir mit Naturbau. Die 
Bienen müssen sich also ihre Behausung selbst ausbauen. Das bedeutet natürlich, dass der Schwarm erst mal bauen muss, bevor 
mit Honigeinlagerung und dem Brüten begonnen werden kann. Wir vermuten, dass einfach einige Bienen im Weg stehen und das der 
Grund für den “Ausflug” ist. Wir werden berichten, ob wir richtig gelegen sind.

![](http://bienenimgarten.files.wordpress.com/2013/06/2013-06-16-08-51-09.jpg?w=960)

Das war ein ereignisreicher Abend. Wir sind todmüde ins Bett gefallen und sind – zugegeben – auch ein wenig Stolz auf 
unseren ersten, selbst gefangenen Bienenschwarm.
"
				};

				this.BlogItems.Add(newItem);
				this.SaveChanges();
			}

			if (!existingBlogIds.Contains(2))
			{
				var newItem = new BlogItem()
				{
					BlogId = 2,
					Title = "Türe auf – eine vergrößerte Fluglochöffnung muss her",
					FeaturedImageUri = "http://bienenimgarten.files.wordpress.com/2013/06/img_0804.jpg?w=500",
					Content = @"
Türe auf!
---------

Endlich die erste richtig lange Schönwetterperiode. Die Bienen entwickeln sich im Moment prächtig. Vor dem Flugloch geht 
es so richtig rund. Bei unserem gestrigen Bienenbesuch haben wir bemerkt, dass die kleine Fluglochöffnung im Moment ein 
echter Engpass ist. Die Bienen müssen sich zum Rein- und Rausgehen richtig anstellen. Aus diesem Grund haben wir heute 
einen Fluglochkeil mit größerer Öffnung gebastelt. Am Foto unten (anklicken zum Vergrößern) sieht man den Unterschied. 
Der obere Fluglochkeil ist der, den man mit der Dadant-Beute von der Firma Janisch bekommt. Den unteren haben wir heute 
gebaut. Im Sommer werden wir den vergrößerten verwenden. Der kleine passt für den Herbst und Winter dann sicher wieder optimal.

![](http://bienenimgarten.files.wordpress.com/2013/06/dscf3103.jpg?w=960)

Die Sache mit der Größe des Fluglochs ist in Imkerkreisen eine vieldiskutierte. Einerseits will man den Bienen genug 
Platz geben. Andererseits macht ein zu großes Flugloch es der Fluglochwache schwer, den Stock gegen ungebetene Gäste wie 
z.B. Spitzmäuse zu verteidigen. Aus diesem Grund haben wir den oben gezeigten Kompromiss gewählt. Das Loch ist zwar über 
die ganze Breite offen, aber nur wenige mm hoch.
"
				};

				this.BlogItems.Add(newItem);
				this.SaveChanges();
			}

			if (!existingBlogIds.Contains(1))
			{
				var newItem = new BlogItem()
				{
					BlogId = 1,
					Title = "Es brummt im Bienenstock",
					FeaturedImageUri = "http://bienenimgarten.files.wordpress.com/2013/06/dscf3078.jpg?w=500",
					Content = @"
In einem unserer letzten Blogartikel haben wir berichtet, dass unsere Königin (die sogenannte Weisel) tot ist. Das war ein 
herber Rückschlag, da wir nicht wussten, ob eine zweite im Volk ist. Heute haben wir tolle Neuigkeiten: Es ist so gut wie 
sicher, dass unser Volk nicht weisellos ist. Herausgefunden haben wir das mit Hilfe unseres Imkereivereinobmanns. Wir haben 
gemeinsam den Stock geöffnet und uns die Brut angesehen. Die tote Königin haben wir vor ziemlich genau einer Woche gefunden. 
In den Wabenzellen fanden wir aber gestern sowohl Eier als auch Bienenlarven, die teilweise erst wenige Tage alt waren. Das ist 
nur möglich mit einer aktiven Königin.

Darüber hinaus waren wir begeistert, wie fleißig unser Bienenvolk bereits gebaut hat. Das folgende Foto zeigt eine der 
Brutwaben, die die Bienen in ziemlich genau eineinhalb Wochen im Naturbau gebaut haben (anklicken zum Vergrößern):

![](http://bienenimgarten.files.wordpress.com/2013/06/dscf3073.jpg?w=960)

Es scheint also im Moment alles in Ordnung mit unserem Bienenvolk zu sein. Wir werden trotzdem die nächsten Tage noch 
weiter mit Zuckerwasser füttern müssen. Das Wetter ist trotz Anfang Juni immer noch katastrophal. Es regnet bei weniger als 
15° voraussichtlich noch bis Mitte nächster Woche. Danach hoffen wir (und unsere Bienen), dass es endlich richtig Sommer wird.

Beim Kontrollieren der Beute konnte ich noch ein paar gute Fotos machen. Die möchte ich hier mit euch teilen 
(anklicken zum Vergrößern):

![](http://bienenimgarten.files.wordpress.com/2013/06/dscf3077.jpg?w=300&h=199) ![](http://bienenimgarten.files.wordpress.com/2013/06/dscf3079.jpg?w=300&h=199)

Zurück auf den Balkon?
----------------------

Wie berichtet hatten wir das Problem, als die Bienen noch auf dem Balkon standen, dass sie sich in großer Zahl zu unseren 
Nachbarn verflogen haben. Was dieses Problem betrifft, haben wir weiter recherchiert.

Ein Tipp, den wir bekommen haben, bezog sich auf die Positionierung und Farbe der Beute. Wir hatten am Balkon unsere Beute 
bewusst so hingestellt, dass die Bienen über das Balkongeländer fliegen mussten. Unser Ziel war, dafür zu sorgen, dass sie 
nach oben weg fliegen und so die Nachbarn wenig von den Bienen zu spüren bekommen. Vielleicht war das ein Fehler. Die Beute 
war so von außen schwer zu sehen, da sie zum Großteil hinter dem Balkongeländer verschwand. Der Tipp war, die Beute erhöht 
aufzustellen und ganz nach vorne zu schieben, sodass ein Teil (z.B. das Anflugbrett) über das Geländer hinaus schaut. Die Beute 
sollte dann von außen gut sichtbar sein.

Die zweite Maßnahme, die hilfreich sein könnte, wäre eine farbliche Markierung. Wir haben uns schlau gemacht, was 
die Farb- und Mustererkennung von Bienen betrifft. Für alle Leser, die an diesem Thema ebenfalls interessiert sind, 
empfehlen wir die folgenden Artikel:
* [Das komplexe Denken der Bienen](http://sciencev1.orf.at/science/news/10903), in ORF On Science
* Menzel R.: [Farbsehen blütenbesuchender Insekten](http://www.neurobiologie.fu-berlin.de/menzel/Pub_AGmenzel/Menzel_JB%20KFA%20J%C3%BClich_1987_100dpi.pdf), Institut 
für Neurobiologie der freien Universität Berlin

Vielleicht wäre es hilfreich, nicht nur die Beute zu markieren sondern auch den Balkon selbst mit Blumenkästen in, für 
Bienen gut wahrnehmbaren Farben und vielleicht sogar mit UV-reflektierenden Markern von weitem erkennbar zu machen.

Im Moment bleiben unsere Bienen im Garten. Wir warten auf jeden Fall die jetzt (hoffentlich) kommende, erste warme Periode 
des Sommers 2013 ab. Unsere Bienen sollen die nächsten Wochen in Ruhe an ihren Waben bauen. Danach werden wir vielleicht 
nochmals ganz vorsichtig bei unseren Nachbarn anklopfen und fragen, ob wir einen zweiten Versuch am Balkon wagen dürfen. 
Parallel dazu arbeiten wir auch an einer zweiten Variante, über die wir an anderer Stelle berichten werden.
"
				};

				this.BlogItems.Add(newItem);
				this.SaveChanges();
			}
		}
	}
}
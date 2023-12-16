type Attribute = {
  name: string
  description: string
  detail: string
  empty?: boolean
}

type Element = {
  name: string
  detail: string
  description: string
  autoClose?: boolean
  elements: string[]
  attributes: Attribute[]
}

const commonElements = [
  "Row",
  "Column",
  "Text",
  "Table",
  "Image",
  "LineVertical",
  "LineHorizontal",
  "Inline",
  "Virtual",
  "PageBreak",
  "Template"
]

const commonAttributes: Attribute[] = [
  {
    name: "if",
    description: "Condition to show the element.",
    detail: "conditional attribute"
  },
  {
    name: "else",
    description: "Condition to show if the if is false.",
    detail: "conditional attribute",
    empty: true
  },
  {
    name: "repeat",
    description: "Number for how many times the element should repeat. Elements are provided the `{index}` token",
    detail: "loop attribute"
  },
  {
    name: "orderByAsc",
    description: "Orders a `for` loop ascending by the given key",
    detail: "loop attribute"
  },
  {
    name: "orderByDesc",
    description: "Orders a `for` loop descending by the given key",
    detail: "loop attribute"
  },
  {
    name: "bind",
    description: "Binds a set of data to an element. Like a for without repeating",
    detail: "loop attribute"
  },
  {
    name: "for",
    description: "Statement on repeating over a set. For example `row in Customer`, `row in SELECT TOP 10 FROM Customer` or `row in {customer}`",
    detail: "loop attribute"
  },
  {
    name: "groupBy",
    description: "Used only with the `for` attribute. String key to group the for sets by.",
    detail: "loop attribute"
  },
  {
    name: "fontFamily",
    description: "Name of font family to use. Must be installed on system running the service",
    detail: "font attribute"
  },
  {
    name: "fontColour",
    description: "Colour of the font. Hex code or common HTML colour code (US spelling)",
    detail: "font attribute"
  },
  {
    name: "fontSize",
    description: "Point size of the fonts. Different fonts may have different sizes",
    detail: "font attribute"
  },
  {
    name: "letterSpacing",
    description: "Point size increase from the default spacing between letters",
    detail: "font attribute"
  },
  {
    name: "lineHeight",
    description: "Point size of space between lines",
    detail: "font attribute"
  },
  {
    name: "fontEmphasis",
    description: "Emphasis for fonts. Can be space separated",
    detail: "font attribute"
  },
  {
    name: "padding",
    description: "Point size padding surrounding the element",
    detail: "layout attribute"
  },
  {
    name: "paddingTop",
    description: "Point size padding to the top the element",
    detail: "layout attribute"
  },
  {
    name: "paddingBottom",
    description: "Point size padding to the bottom the element",
    detail: "layout attribute"
  },
  {
    name: "paddingLeft",
    description: "Point size padding to the left the element",
    detail: "layout attribute"
  },
  {
    name: "paddingRight",
    description: "Point size padding to the right of the element",
    detail: "layout attribute"
  },
  {
    name: "paddingVertical",
    description: "Point size padding to the vertical sides of the element",
    detail: "layout attribute"
  },
  {
    name: "paddingHorizontal",
    description: "Point size padding to the horizontal sides of the element",
    detail: "layout attribute"
  },
  {
    name: "border",
    description: "Decimal point width for the border",
    detail: "style attribute"
  },
  {
    name: "borderLeft",
    description: "Decimal point width for the left border",
    detail: "style attribute"
  },
  {
    name: "borderRight",
    description: "Decimal point width for the right border",
    detail: "style attribute"
  },
  {
    name: "borderTop",
    description: "Decimal point width for the top border",
    detail: "style attribute"
  },
  {
    name: "borderBottom",
    description: "Decimal point width for the bottom border",
    detail: "style attribute"
  },
  {
    name: "borderVertical",
    description: "Decimal point width for the vertical borders",
    detail: "style attribute"
  },
  {
    name: "borderHorizontal",
    description: "Decimal point width for the horizontal borders",
    detail: "style attribute"
  },
  { 
    name: "borderColor",
    description: "Color of the border. Hex code or common HTML colour code (US spelling)",
    detail: "style attribute"
  },
  {
    name: "debug",
    description: "Displays a debug overlay with this name",
    detail: "debug attribute"
  },
  {
    name: "width",
    description: "Decimal point width for the container. Might error if there is not enough space to fit contents",
    detail: "layout attribute"
  },
  {
    name: "height",
    description: "Decimal point height for the container. Might error if there is not enough space to fit contents",
    detail: "layout attribute"
  },
  {
    name: "minHeight",
    description: "Decimal point minimal height for the container. Due to the flexible layout this normally doesn't work well",
    detail: "layout attribute"
  },
  {
    name: "maxHeight",
    description: "Decimal point maximum height for the container. Due to the flexible layout this normally doesn't work well",
    detail: "layout attribute"
  },
  {
    name: "minWidth",
    description: "Decimal point minimal width for the container. Due to the flexible layout this normally doesn't work well",
    detail: "layout attribute"
  },
  {
    name: "maxWidth",
    description: "Decimal point maximum width for the container. Due to the flexible layout this normally doesn't work well",
    detail: "layout attribute"
  },
  {
    name: "align",
    description: "Alignment of the container. Can be space delimited. `top`, `bottom`, `right`, `left`, `center`, `middle`",
    detail: "layout attribute"
  },
  {
    name: "minimalBox",
    description: "Causes the container to take up the minimal amount of space. No value required",
    detail: "layout attribute",
    empty: true
  },
  {
    name: "unconstrained",
    description: "Removes all constraints from the layout. No value required",
    detail: "layout attribute",
    empty: true
  },
  {
    name: "background",
    description: "Colour of the background. Hex code or common HTML colour code (US spelling)",
    detail: "layout attribute"
  },
  {
    name: "extend",
    description: "Extends the container in a direction. `vertical`, `horizontal` or `both`",
    detail: "layout attribute"
  },
  {
    name: "columnSpan",
    description: "Number of columns to span over. Only used for direct children of `TableRow`, `TableHeader` or `TableFooter`",
    detail: "layout attribute"
  },
  {
    name: "rowSpan",
    description: "Number of rows to span over. Only used for direct children of `TableRow`, `TableHeader` or `TableFooter`",
    detail: "layout attribute"
  },
  {
    name: "columnWidth",
    description: "Width of the column in format `relative,x` or `constant,x` where `x` is the relative width of the table or decimal point width. Only used for direct children of `TableRow`, `TableHeader` or `TableFooter`",
    detail: "layout attribute"
  },
  {
    name: "showOnce",
    description: "Shows this element once if the page is to be repeated. No value required",
    detail: "layout attribute",
    empty: true
  },
  {
    name: "template",
    description: "Name of the template this element should inherit.",
    detail: "layout attribute"
  },
]

const elements : Record<string, Element> = {
  Document: {
    name: "Document",
    detail: "basic component",
    description: "The document that contains pages.",
    elements: [
      "Page"
    ],
    attributes: []
  },
  Page: {
    name: "Page",
    detail: "basic component",
    description: "Represents a page of the document. Contains header, footer and content.",
    elements: [
      "Header",
      "Footer",
      "Content"
    ],
    attributes: [
      {
        name: "margin",
        description: "Decimal point size for page margin",
        detail: "page attribute"
      },
      {
        name: "orientation",
        description: "Page orientation. `portrait` or `landscape`",
        detail: "page attribute"
      },
      {
        name: "size",
        description: "Size of the page, in inches, comma separated. e.g. `6,4` for a 6 x 4 inch label",
        detail: "page attribute"
      },
      ...commonAttributes
    ]
  },
  Header: {
    name: "Header",
    detail: "basic component",
    description: "Repeats at the top of contained pages. Can only have one single child",
    elements: commonElements,
    attributes: [
      ...commonAttributes
    ]
  },
  Footer: {
    name: "Footer",
    detail: "basic component",
    description: "Repeats at the bottom of all contained pages. Can only have one single child",
    elements: commonElements,
    attributes: [
      ...commonAttributes
    ]
  },
  Content: {
    name: "Content",
    detail: "basic component",
    description: "Contains the main body of content for the page. Can only have one single child",
    elements: commonElements,
    attributes: [
      ...commonAttributes
    ]
  },
  Column: {
    name: "Column",
    detail: "basic component",
    description: "A column of content, child elements will be arranged vertically",
    elements: commonElements,
    attributes: [
      {
        name: "spacing",
        description: "Decimal point spacing between child elements",
        detail: "spacing attribute"
      },
      ...commonAttributes
    ]
  },
  Row: {
    name: "Row",
    detail: "basic component",
    description: "A row of content, child elements will be arranged horizontally",
    elements: commonElements,
    attributes: [
      {
        name: "spacing",
        description: "Decimal point spacing between child elements",
        detail: "spacing attribute"
      },
      ...commonAttributes
    ]
  },
  Text: {
    name: "Text",
    detail: "basic component",
    description: "A basic text item.",
    elements: [],
    attributes: [
      ...commonAttributes
    ]
  },
  Table: {
    name: "Table",
    detail: "basic component",
    description: "A basic text item.",
    elements: [
      "TableRow",
      "TableHeader",
      "TableFooter",
      "Virtual"
    ],
    attributes: [
      ...commonAttributes
    ]
  },
  TableRow: {
    name: "TableRow",
    detail: "basic component",
    description: "A table row _must_ be the child of a `Table` or `Virtual` container",
    elements: [
      ...commonElements
    ],
    attributes: [
      ...commonAttributes
    ]
  },
  TableHeader: {
    name: "TableFooter",
    detail: "basic component",
    description: "A table header that repeats each page of the table. _Must_ be the child of a `Table`",
    elements: [
      ...commonElements
    ],
    attributes: [
      ...commonAttributes
    ]
  },
  TableFooter: {
    name: "TableFooter",
    detail: "basic component",
    description: "A table footer that repeats each page of the table. _Must_ be the child of a `Table`",
    elements: [
      ...commonElements
    ],
    attributes: [
      ...commonAttributes
    ]
  },
  Image: {
    name: "Image",
    detail: "basic component",
    description: "For displaying an image",
    autoClose: true,
    elements: [
      ...commonElements
    ],
    attributes: [
      {
        name: "scaling",
        description: "How the image scales in relation to the container. `width`, `height` or 'area'",
        detail: "layout attribute"
      },
      {
        name: "src",
        description: "Disk location of the image relative to the service host",
        detail: "layout attribute"
      },
      ...commonAttributes
    ]
  },
  Template: {
    name: "Template",
    detail: "basic component",
    description: "A template that can be referenced by other components",
    autoClose: true,
    elements: [
      ...commonElements
    ],
    attributes: [
      {
        name: "name",
        description: "Name of the template",
        detail: "template attribute"
      },
      ...commonAttributes
    ]
  },
  LineVertical: {
    name: "LineVertical",
    detail: "basic component",
    description: "For displaying a vertical line across the height of the container",
    autoClose: true,
    elements: [
      ...commonElements
    ],
    attributes: [
      {
        name: "colour",
        description: "Colour of the line. Hex code or common HTML colour code (US spelling)",
        detail: "layout attribute"
      },
      {
        name: "lineWidth",
        description: "Decimal point width of the line",
        detail: "layout attribute"
      },
      ...commonAttributes
    ]
  },
  LineHorizontal: {
    name: "LineHorizontal",
    detail: "basic component",
    description: "For displaying a horizontal line across the height of the container",
    autoClose: true,
    elements: [
      ...commonElements
    ],
    attributes: [
      {
        name: "colour",
        description: "Colour of the line. Hex code or common HTML colour code (US spelling)",
        detail: "layout attribute"
      },
      {
        name: "lineWidth",
        description: "Decimal point width of the line",
        detail: "layout attribute"
      },
      ...commonAttributes
    ]
  },
  Inline: {
    name: "Inlined",
    detail: "basic component",
    description: "An inline container",
    elements: [
      ...commonElements
    ],
    attributes: [
      ...commonAttributes
    ]
  },
  Virtual: {
    name: "Virtual",
    detail: "basic component",
    description: "A virtual container that is dissolved during expansion",
    elements: [
      "TableRow",
      "TableHeader",
      "TableFooter",
      ...commonElements
    ],
    attributes: [
      ...commonAttributes
    ]
  },
  PageBreak: {
    name: "PageBreak",
    detail: "basic component",
    description: "Forces a page break at the current location",
    autoClose: true,
    elements: [
      ...commonElements
    ],
    attributes: [
      ...commonAttributes
    ]
  },
}

export default elements
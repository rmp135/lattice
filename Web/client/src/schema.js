const commonElements = [
  "Row",
  "Column",
  "Text",
  "Table",
  "Grid",
  "Image",
  "LineVertical",
  "LineHorizontal",
  "Inline",
  "Virtual",
  "PageBreak"
]

const commonAttributes = [
  {
    "name": "if",
    "description": "Condition to show the element.",
    "detail": "conditional attribute",
    "options": null
  },
  {
    "name": "repeat",
    "description": "Number for how many times the element should repeat. Elements are provided the `{index}` token",
    "detail": "loop attribute",
    "options": null
  },
  {
    "name": "orderByAsc",
    "description": "Orders a `for` loop ascending by the given key",
    "detail": "loop attribute",
    "options": null
  },
  {
    "name": "orderByDesc",
    "description": "Orders a `for` loop descending by the given key",
    "detail": "loop attribute",
    "options": null
  },
  {
    "name": "bind",
    "description": "Binds a set of data to an element. Like a for without repeating",
    "detail": "loop attribute",
    "options": null
  },
  {
    "name": "for",
    "description": "Statement on repeating over a set. For example `row in Customer`, `row in SELECT TOP 10 FROM Customer` or `row in {customer}`",
    "detail": "loop attribute",
    "options": null
  },
  {
    "name": "groupBy",
    "description": "Used only with the `for` attribute. String key to group the for sets by.",
    "detail": "loop attribute",
    "options": null
  },
  {
    "name": "fontFamily",
    "description": "Name of font family to use. Must be installed on system running the service",
    "detail": "font attribute",
    "options": null
  },
  {
    "name": "fontColour",
    "description": "Colour of the font. Hex code or common HTML colour code (US spelling)",
    "detail": "font attribute",
    "options": null
  },
  {
    "name": "fontSize",
    "description": "Point size of the fonts. Different fonts may have different sizes",
    "detail": "font attribute",
    "options": null
  },
  {
    "name": "letterSpacing",
    "description": "Point size increase from the default spacing between letters",
    "detail": "font attribute",
    "options": null
  },
  {
    "name": "lineHeight",
    "description": "Point size of space between lines",
    "detail": "font attribute",
    "options": null
  },
  {
    "name": "fontEmphasis",
    "description": "Emphasis for fonts. Can be space separated",
    "detail": "font attribute",
    "options": [
      "italic",
      "thin",
      "extraLight",
      "light",
      "medium",
      "semiBold",
      "bold",
      "extraBold",
      "black",
      "extraBlack",
      "underline",
      "strikethrough",
      "superscript",
      "subscript"
    ]
  },
  {
    "name": "padding",
    "description": "Point size padding surrounding the element",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "paddingTop",
    "description": "Point size padding to the top the element",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "paddingBottom",
    "description": "Point size padding to the bottom the element",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "paddingLeft",
    "description": "Point size padding to the left the element",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "paddingRight",
    "description": "Point size padding to the right of the element",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "paddingVertical",
    "description": "Point size padding to the vertical sides of the element",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "paddingHorizontal",
    "description": "Point size padding to the horizontal sides of the element",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "border",
    "description": "Decimal point width for the border",
    "detail": "style attribute",
    "options": []
  },
  {
    "name": "borderLeft",
    "description": "Decimal point width for the left border",
    "detail": "style attribute",
    "options": []
  },
  {
    "name": "borderRight",
    "description": "Decimal point width for the right border",
    "detail": "style attribute",
    "options": []
  },
  {
    "name": "borderTop",
    "description": "Decimal point width for the top border",
    "detail": "style attribute",
    "options": []
  },
  {
    "name": "borderBottom",
    "description": "Decimal point width for the bottom border",
    "detail": "style attribute",
    "options": []
  },
  {
    "name": "borderVertical",
    "description": "Decimal point width for the vertical borders",
    "detail": "style attribute",
    "options": []
  },
  {
    "name": "borderHorizontal",
    "description": "Decimal point width for the horizontal borders",
    "detail": "style attribute",
    "options": []
  },
  { 
    "name": "borderColor",
    "description": "Color of the border. Hex code or common HTML colour code (US spelling)",
    "detail": "style attribute",
    "options": []
  },
  {
    "name": "debug",
    "description": "Displays a debug overlay with this name",
    "detail": "debug attribute",
    "options": []
  },
  {
    "name": "width",
    "description": "Decimal point width for the container. Might error if there is not enough space to fit contents",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "height",
    "description": "Decimal point height for the container. Might error if there is not enough space to fit contents",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "minHeight",
    "description": "Decimal point minimal height for the container. Due to the flexible layout this normally doesn't work well",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "maxHeight",
    "description": "Decimal point maximum height for the container. Due to the flexible layout this normally doesn't work well",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "minWidth",
    "description": "Decimal point minimal width for the container. Due to the flexible layout this normally doesn't work well",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "maxWidth",
    "description": "Decimal point maximum width for the container. Due to the flexible layout this normally doesn't work well",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "align",
    "description": "Alignment of the container. Can be space delimited. `top`, `bottom`, `right`, `left`, `center`, `middle`",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "minimalBox",
    "description": "Causes the container to take up the minimal amount of space. No value required",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "unconstrained",
    "description": "Removes all constraints from the layout. No value required",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "background",
    "description": "Colour of the background. Hex code or common HTML colour code (US spelling)",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "extend",
    "description": "Extends the container in a direction. `vertical`, `horizontal` or `both`",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "columnSpan",
    "description": "Number of columns to span over. Only used for direct children of `TableRow`, `TableHeader` or `TableFooter`",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "rowSpan",
    "description": "Number of rows to span over. Only used for direct children of `TableRow`, `TableHeader` or `TableFooter`",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "columnWidth",
    "description": "Width of the column in format `relative,x` or `constant,x` where `x` is the relative width of the table or decimal point width. Only used for direct children of `TableRow`, `TableHeader` or `TableFooter`",
    "detail": "layout attribute",
    "options": []
  },
  {
    "name": "showOnce",
    "description": "Shows this element once if the page is to be repeated. No value required",
    "detail": "layout attribute",
    "options": []
  },
]

export default {
  "Document": {
    "name": "Document",
    "detail": "basic component",
    "description": "The document that contains pages.",
    "elements": [
      "Page"
    ],
    "attributes": []
  },
  "Page": {
    "name": "Page",
    "detail": "basic component",
    "description": "Represents a page of the document. Contains header, footer and content.",
    "elements": [
      "Header",
      "Footer",
      "Content"
    ],
    "attributes": [
      {
        "name": "margin",
        "description": "Decimal point size for page margin",
        "detail": "page attribute",
        "options": null
      },
      {
        "name": "orientation",
        "description": "Page orientation. `portrait` or `landscape`",
        "detail": "page attribute",
        "options": null
      },
      {
        "name": "size",
        "description": "Size of the page, in inches, comma separated. e.g. `6,4` for a 6 x 4 inch label",
        "detail": "page attribute",
        "options": [
          "6,4"
        ]
      },
      ...commonAttributes
    ]
  },
  "Header": {
    "name": "Header",
    "detail": "basic component",
    "description": "Repeats at the top of contained pages. Can only have one single child",
    "elements": commonElements,
    "attributes": [
      ...commonAttributes
    ]
  },
  "Footer": {
    "name": "Footer",
    "detail": "basic component",
    "description": "Repeats at the bottom of all contained pages. Can only have one single child",
    "elements": commonElements,
    "attributes": [
      ...commonAttributes
    ]
  },
  "Content": {
    "name": "Content",
    "detail": "basic component",
    "description": "Contains the main body of content for the page. Can only have one single child",
    "elements": commonElements,
    "attributes": [
      ...commonAttributes
    ]
  },
  "Column": {
    "name": "Column",
    "detail": "basic component",
    "description": "A column of content, child elements will be arranged vertically",
    "elements": commonElements,
    "attributes": [
      {
        "name": "spacing",
        "description": "Decimal point spacing between child elements",
        "detail": "spacing attribute",
        "options": null
      },
      ...commonAttributes
    ]
  },
  "Row": {
    "name": "Row",
    "detail": "basic component",
    "description": "A row of content, child elements will be arranged horizontally",
    "elements": commonElements,
    "attributes": [
      {
        "name": "spacing",
        "description": "Decimal point spacing between child elements",
        "detail": "spacing attribute",
        "options": null
      },
      ...commonAttributes
    ]
  },
  "Text": {
    "name": "Text",
    "detail": "basic component",
    "description": "A basic text item.",
    "elements": [],
    "attributes": [
      ...commonAttributes
    ]
  },
  "Table": {
    "name": "Table",
    "detail": "basic component",
    "description": "A basic text item.",
    "elements": [
      "TableRow",
      "TableHeader",
      "TableFooter",
      "Virtual"
    ],
    "attributes": [
      ...commonAttributes
    ]
  },
  "TableRow": {
    "name": "TableRow",
    "detail": "basic component",
    "description": "A table row _must_ be the child of a `Table` or `Virtual` container",
    "elements": [
      ...commonElements
    ],
    "attributes": [
      ...commonAttributes
    ]
  },
  "TableHeader": {
    "name": "TableFooter",
    "detail": "basic component",
    "description": "A table header that repeats each page of the table. _Must_ be the child of a `Table`",
    "elements": [
      ...commonElements
    ],
    "attributes": [
      ...commonAttributes
    ]
  },
  "TableFooter": {
    "name": "TableFooter",
    "detail": "basic component",
    "description": "A table footer that repeats each page of the table. _Must_ be the child of a `Table`",
    "elements": [
      ...commonElements
    ],
    "attributes": [
      ...commonAttributes
    ]
  },
  "Grid": {
    "name": "Grid",
    "detail": "basic component",
    "description": "For aligning items in a 12 wide grid span",
    "elements": [
      ...commonElements
    ],
    "attributes": [
      {
        "name": "widths",
        "description": "Relative widths of columns in 12ths. `8,4,6,6` will create two rows; the first two cells of 8 12ths and 4ths, and the second 6 12ths and 6ths.",
        "detail": "layout attribute",
        "options": null
      },
      ...commonAttributes
    ]
  },
  "Image": {
    "name": "Image",
    "detail": "basic component",
    "description": "For displaying an image",
    "autoClose": true,
    "elements": [
      ...commonElements
    ],
    "attributes": [
      {
        "name": "scaling",
        "description": "How the image scales in relation to the container. `width`, `height` or 'area'",
        "detail": "layout attribute",
        "options": null
      },
      {
        "name": "src",
        "description": "Disk location of the image relative to the service host",
        "detail": "layout attribute",
        "options": null
      },
      {
        "name": "widths",
        "description": "Relative widths of columns in 12ths. `8,4,3,9` will create two rows; the first two cells of 8 12ths and 4ths, and the second 3 12ths and 9 12ths.",
        "detail": "layout attribute",
        "options": null
      },
      ...commonAttributes
    ]
  },
  "LineVertical": {
    "name": "LineVertical",
    "detail": "basic component",
    "description": "For displaying a vertical line across the height of the container",
    "autoClose": true,
    "elements": [
      ...commonElements
    ],
    "attributes": [
      {
        "name": "colour",
        "description": "Colour of the line. Hex code or common HTML colour code (US spelling)",
        "detail": "layout attribute",
        "options": null
      },
      {
        "name": "lineWidth",
        "description": "Decimal point width of the line",
        "detail": "layout attribute",
        "options": null
      },
      ...commonAttributes
    ]
  },
  "LineHorizontal": {
    "name": "LineHorizontal",
    "detail": "basic component",
    "description": "For displaying a horizontal line across the height of the container",
    "autoClose": true,
    "elements": [
      ...commonElements
    ],
    "attributes": [
      {
        "name": "colour",
        "description": "Colour of the line. Hex code or common HTML colour code (US spelling)",
        "detail": "layout attribute",
        "options": null
      },
      {
        "name": "lineWidth",
        "description": "Decimal point width of the line",
        "detail": "layout attribute",
        "options": null
      },
      ...commonAttributes
    ]
  },
  "Inline": {
    "name": "Inlined",
    "detail": "basic component",
    "description": "An inline container",
    "elements": [
      ...commonElements
    ],
    "attributes": [
      ...commonAttributes
    ]
  },
  "Virtual": {
    "name": "Virtual",
    "detail": "basic component",
    "description": "A virtual container that is dissolved during expansion",
    "elements": [
      "TableRow",
      "TableHeader",
      "TableFooter",
      ...commonElements
    ],
    "attributes": [
      ...commonAttributes
    ]
  },
  "PageBreak": {
    "name": "PageBreak",
    "detail": "basic component",
    "description": "Forces a page break at the current location",
    "autoClose": true,
    "elements": [
      ...commonElements
    ],
    "attributes": [
      ...commonAttributes
    ]
  },
}

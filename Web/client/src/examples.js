export default {
    "Blank": `<Document>
  <Page>
    <Content>
    </Content>
  </Page>
</Document>`,
    "Grouping and Aggregation": `<Document>
  <Page margin="20" orientation="landscape">
    <Header fontColour="gray" paddingBottom="10">
      <Text>Periodic Table of Elements</Text>
    </Header>
    <Content>
      <Column>
        <Table>
          <TableHeader background="gray" fontColour="white" border="1" padding="3">
            <Text columnWidth="constant(100)">Atomic Number</Text>
            <Text>Element</Text>
            <Text>Symbol</Text>
            <Text>Phase</Text>
            <Text>Atomic Mass</Text>
          </TableHeader>
          <Virtual for="group in elements" groupBy="Type" orderByAsc="Type">
            <TableRow border="1" background="lightblue" padding="3">
              <Text columnSpan="5">{group.Type} - Count: {count(group)}</Text>
            </TableRow>
            <TableRow for="e in {group}" border="1" padding="3">
              <Text>{e.AtomicNumber}</Text>
              <Text>{e.Element}</Text>
              <Text>{e.Symbol}</Text>
              <Text>{e.Phase}</Text>
              <Text>{e.AtomicMass}</Text>
            </TableRow>
          </Virtual>
        </Table>
      </Column>
    </Content>
    <Footer fontColour="gray">
      <Text align="right">{currentpage} / {totalpages}</Text>
    </Footer>
  </Page>
</Document>`,
    "Grids":`<Document>
  <Page margin="20">
    <Content>
      <Column>
        <Text>Grids are used to align content based on grid widths in a 12 column layout.</Text>
        <Inline paddingBottom="5">
          <Text>The below layout was specified on the grid like this: </Text>
          <Text background="lightblue" > widths="8,4,2,10" </Text>
        </Inline>
        <Grid widths="8,4,2,10" spacing="5">
          <Inline background="blue" fontColour="white" padding="5">
            <Text>8 12</Text>
            <Text fontEmphasis="superscript">ths</Text>
          </Inline>
          <Inline background="red" padding="5">
            <Text>4 12</Text>
            <Text fontEmphasis="superscript">ths</Text>
          </Inline>
          <Inline background="green" fontColour="white" padding="5">
            <Text>2 12</Text>
            <Text fontEmphasis="superscript">ths</Text>
          </Inline>
          <Inline background="orange" padding="5">
            <Text>10 12</Text>
            <Text fontEmphasis="superscript">ths</Text>
          </Inline>
        </Grid>
      </Column>
    </Content>
  </Page>
</Document>`
}
//Creating Comment box form and Related Ad Comments Components

class CommentForm extends React.Component {
    constructor(props) {
      super(props);
        this.state = {value: props.datas};

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        this.setState({value: event.target.value});
    }

    handleSubmit(event) {
        alert('A name was submitted: ' + this.state.value);
        event.preventDefault();
    }

    render() {
        return (

          <form onSubmit={this.handleSubmit}>
            <label>
    Name:
          <input type="text" value={this.props.datas} onChange={this.handleChange} />
        </label>
        <input type="submit" value="Submit" />
      </form>
    );
}
}

ReactDOM.render(
  <CommentForm datas="whatever" />,
  document.getElementById('root')
);
import React, { Component } from 'react';
import { TouchableOpacity, Text, TextInput, StyleSheet, View } from 'react-native';
import { Container, Header, Content, Form, Item, Input, Label, DatePicker, Picker, Icon } from 'native-base';

export default class ServerError extends Component {
  render() {
    return (
      <View style= {styles.container}>
        <Text style= {styles.text}> Server Error </Text>
      </View>
    )
  }
}

const styles = StyleSheet.create({
    container: {
      flex: 1,
      justifyContent: 'center',
      alignItems: 'center',
      alignContent: 'center',
      backgroundColor: "#eeeeee",
    },
    text: {
      color: 'red',
      fontSize: 75,
      fontWeight: 'bold'
    }
});
